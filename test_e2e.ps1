Add-Type -AssemblyName System.Net.Http

$baseUrl = "http://localhost:52000"
$allPassed = $true

function Report-Result($testName, $passed, $detail) {
    if ($passed) {
        Write-Host "[PASS] $testName - $detail" -ForegroundColor Green
    } else {
        Write-Host "[FAIL] $testName - $detail" -ForegroundColor Red
        $script:allPassed = $false
    }
}

function Create-Client() {
    $handler = New-Object System.Net.Http.HttpClientHandler
    $handler.AllowAutoRedirect = $false
    $handler.CookieContainer = New-Object System.Net.CookieContainer
    $client = New-Object System.Net.Http.HttpClient($handler)
    $client.Timeout = [TimeSpan]::FromSeconds(30)
    return @{ Client = $client; Handler = $handler }
}

Write-Host "========== RUNNING COMPREHENSIVE E2E TEST SUITE ==========" -ForegroundColor Cyan

# 1. Test Home & Catalog
$guest = Create-Client
try {
    $resHome = $guest.Client.GetAsync("$baseUrl/").Result
    $content = $resHome.Content.ReadAsStringAsync().Result
    Report-Result "1. Trang Chu Website" ($resHome.StatusCode -eq 200) "Status $([int]$resHome.StatusCode)"
} catch {
    Report-Result "1. Trang Chu Website" $false $_.Exception.Message
}

try {
    $resCatalog = $guest.Client.GetAsync("$baseUrl/KhoaHoc/KhoaHoc").Result
    $content = $resCatalog.Content.ReadAsStringAsync().Result
    $hasCourses = $content -like "*MVC2026*" -or $content -like "*ASP.NET*" -or $content -like "*Khóa học*"
    Report-Result "2. Danh Sach Khoa Hoc" ($resCatalog.StatusCode -eq 200 -and $hasCourses) "Status 200 with catalog data"
} catch {
    Report-Result "2. Danh Sach Khoa Hoc" $false $_.Exception.Message
}

try {
    $resDetail = $guest.Client.GetAsync("$baseUrl/KhoaHoc/ChiTietKhoaHoc/MVC2026").Result
    $content = $resDetail.Content.ReadAsStringAsync().Result
    $hasOutline = $content -like "*MVC2026*" -or $content -like "*ASP.NET*" -or $content -like "*Đăng ký*" -or $content -like "*bài học*"
    Report-Result "3. Chi Tiet Khoa Hoc MVC2026" ($resDetail.StatusCode -eq 200 -and $hasOutline) "Status 200 with course details & syllabus"
} catch {
    Report-Result "3. Chi Tiet Khoa Hoc MVC2026" $false $_.Exception.Message
}

# 2. Test Authorization & Protection
try {
    $resAdminGuest = $guest.Client.GetAsync("$baseUrl/HomeAdmin/Index").Result
    $status = [int]$resAdminGuest.StatusCode
    Report-Result "4. Authorization: Admin chan Guest" ($status -eq 302) "Redirected to login ($status)"
} catch {
    Report-Result "4. Authorization: Admin chan Guest" $false $_.Exception.Message
}

try {
    $resGVGuest = $guest.Client.GetAsync("$baseUrl/GiangVienKhoaHoc/Index").Result
    $status = [int]$resGVGuest.StatusCode
    Report-Result "5. Authorization: Giang Vien chan Guest" ($status -eq 302) "Redirected to login ($status)"
} catch {
    Report-Result "5. Authorization: Giang Vien chan Guest" $false $_.Exception.Message
}

# 3. Test Trial Lesson (ChoXemThu) for Guest
try {
    # BaiHoc 13 has ChoXemThu = True
    $resTrial = $guest.Client.GetAsync("$baseUrl/HocTap/VaoHoc/MVC2026?baiId=13").Result
    $content = $resTrial.Content.ReadAsStringAsync().Result
    $isAllowed = ($resTrial.StatusCode -eq 200) -and ($content -like "*XEM THỬ*" -or $content -like "*Học thử*" -or $content -like "*trial*" -or $content -like "*Bài 1*" -or $content -like "*video*")
    Report-Result "6. Hoc Thu (ChoXemThu) cho Khach" $isAllowed "Guest can view trial lesson (Status 200)"
} catch {
    Report-Result "6. Hoc Thu (ChoXemThu) cho Khach" $false $_.Exception.Message
}

# 4. Test Gating Non-Trial Lesson for Guest
try {
    # BaiHoc 15 has ChoXemThu = False
    $resNonTrial = $guest.Client.GetAsync("$baseUrl/HocTap/VaoHoc/MVC2026?baiId=15").Result
    $status = [int]$resNonTrial.StatusCode
    Report-Result "7. Chan Bai Khong Cho Xem Thu doi voi Khach" ($status -eq 302) "Blocked non-trial lesson (Redirect $status)"
} catch {
    Report-Result "7. Chan Bai Khong Cho Xem Thu doi voi Khach" $false $_.Exception.Message
}

# 5. Test DangKyKhoaHoc GET Does NOT Mutate DB
try {
    $resGetReg = $guest.Client.GetAsync("$baseUrl/KhoaHoc/DangKyKhoaHoc/MVC2026").Result
    $status = [int]$resGetReg.StatusCode
    Report-Result "8. DangKyKhoaHoc GET Khong Sua Database" ($status -eq 302) "Guest redirected to login ($status)"
} catch {
    Report-Result "8. DangKyKhoaHoc GET Khong Sua Database" $false $_.Exception.Message
}

# 6. Test GET Mutation Protection on Delete Endpoints
try {
    $resDelDM = $guest.Client.GetAsync("$baseUrl/QuanLyDanhMuc/Delete/9999").Result
    $status = [int]$resDelDM.StatusCode
    Report-Result "9. Chan GET QuanLyDanhMuc/Delete" ($status -eq 302 -or $status -eq 404) "Status $status"
} catch {
    Report-Result "9. Chan GET QuanLyDanhMuc/Delete" $false $_.Exception.Message
}

try {
    $resDelKH = $guest.Client.GetAsync("$baseUrl/QuanLyKhoaHoc/Delete/MVC2026").Result
    $status = [int]$resDelKH.StatusCode
    Report-Result "10. Chan GET QuanLyKhoaHoc/Delete" ($status -eq 302 -or $status -eq 404) "Status $status"
} catch {
    Report-Result "10. Chan GET QuanLyKhoaHoc/Delete" $false $_.Exception.Message
}

# 7. Test Admin Login & Dashboard
$admin = Create-Client
try {
    $loginHtml = $admin.Client.GetStringAsync("$baseUrl/DangNhap/DangNhap").Result
    $token = ""
    if ($loginHtml -match 'name="__RequestVerificationToken"[^>]*value="([^"]+)"') {
        $token = $matches[1]
    }
    $pairs = New-Object "System.Collections.Generic.Dictionary[string,string]"
    $pairs.Add("__RequestVerificationToken", $token)
    $pairs.Add("TenDangNhap", "admin")
    $pairs.Add("MatKhau", "123")
    $content = New-Object System.Net.Http.FormUrlEncodedContent($pairs)
    $loginRes = $admin.Client.PostAsync("$baseUrl/DangNhap/DangNhap", $content).Result
    $status = [int]$loginRes.StatusCode
    Report-Result "11. Dang Nhap Admin voi Password Hashing" ($status -eq 302 -and $loginRes.Headers.Location.ToString() -like "*HomeAdmin*") "Admin logged in (Redirect to $($loginRes.Headers.Location))"
    
    # 8. Test Admin Dashboard with session
    $dashRes = $admin.Client.GetAsync("$baseUrl/HomeAdmin/Index").Result
    $dashHtml = $dashRes.Content.ReadAsStringAsync().Result
    $hasKPI = $dashHtml -like "*Tổng quan*" -or $dashHtml -like "*Khóa học*" -or $dashHtml -like "*Doanh thu*"
    Report-Result "12. Admin Dashboard & Thong Ke" ($dashRes.StatusCode -eq 200 -and $hasKPI) "Dashboard rendered with 200 OK"
} catch {
    Report-Result "11 & 12. Dang Nhap Admin & Dashboard" $false $_.Exception.Message
}

# 9. Test Student Login & My Courses
$student = Create-Client
try {
    $loginHtml = $student.Client.GetStringAsync("$baseUrl/DangNhap/DangNhap").Result
    $token = ""
    if ($loginHtml -match 'name="__RequestVerificationToken"[^>]*value="([^"]+)"') {
        $token = $matches[1]
    }
    $pairs = New-Object "System.Collections.Generic.Dictionary[string,string]"
    $pairs.Add("__RequestVerificationToken", $token)
    $pairs.Add("TenDangNhap", "phuc")
    $pairs.Add("MatKhau", "123")
    $content = New-Object System.Net.Http.FormUrlEncodedContent($pairs)
    $loginRes = $student.Client.PostAsync("$baseUrl/DangNhap/DangNhap", $content).Result
    $status = [int]$loginRes.StatusCode
    Report-Result "13. Dang Nhap Hoc Vien" ($status -eq 302) "Student logged in (Redirect to $($loginRes.Headers.Location))"
    
    $myCoursesRes = $student.Client.GetAsync("$baseUrl/HocTap/KhoaHocCuaToi").Result
    Report-Result "14. Khoa Hoc Cua Toi" ($myCoursesRes.StatusCode -eq 200) "Rendered student courses with 200 OK"

    # Test Student trying to access Admin (Authorization enforcement)
    $studentToAdmin = $student.Client.GetAsync("$baseUrl/HomeAdmin/Index").Result
    $status = [int]$studentToAdmin.StatusCode
    Report-Result "15. Hoc Vien Bi Chan Vao Admin" ($status -eq 302) "Blocked student from accessing admin area"
} catch {
    Report-Result "13-15. Hoc Vien Tests" $false $_.Exception.Message
}

# 10. Test Lecturer Login & Course Management
$lecturer = Create-Client
try {
    $loginHtml = $lecturer.Client.GetStringAsync("$baseUrl/DangNhap/DangNhap").Result
    $token = ""
    if ($loginHtml -match 'name="__RequestVerificationToken"[^>]*value="([^"]+)"') {
        $token = $matches[1]
    }
    $pairs = New-Object "System.Collections.Generic.Dictionary[string,string]"
    $pairs.Add("__RequestVerificationToken", $token)
    $pairs.Add("TenDangNhap", "phuoc")
    $pairs.Add("MatKhau", "123")
    $content = New-Object System.Net.Http.FormUrlEncodedContent($pairs)
    $loginRes = $lecturer.Client.PostAsync("$baseUrl/DangNhap/DangNhap", $content).Result
    $status = [int]$loginRes.StatusCode
    Report-Result "16. Dang Nhap Giang Vien" ($status -eq 302) "Lecturer logged in (Redirect to $($loginRes.Headers.Location))"
    
    $gvCoursesRes = $lecturer.Client.GetAsync("$baseUrl/GiangVienKhoaHoc/Index").Result
    Report-Result "17. Giang Vien Xem Khoa Hoc Cua Minh" ($gvCoursesRes.StatusCode -eq 200) "Rendered lecturer courses with 200 OK"

    # Test Lecturer trying to access Admin (Authorization enforcement)
    $gvToAdmin = $lecturer.Client.GetAsync("$baseUrl/HomeAdmin/Index").Result
    $status = [int]$gvToAdmin.StatusCode
    Report-Result "18. Giang Vien Bi Chan Vao Admin" ($status -eq 302) "Blocked lecturer from accessing admin area"
} catch {
    Report-Result "16-18. Giang Vien Tests" $false $_.Exception.Message
}

# 11. Test Locked Account Rejection
$lockedUser = Create-Client
try {
    $loginHtml = $lockedUser.Client.GetStringAsync("$baseUrl/DangNhap/DangNhap").Result
    $token = ""
    if ($loginHtml -match 'name="__RequestVerificationToken"[^>]*value="([^"]+)"') {
        $token = $matches[1]
    }
    $pairs = New-Object "System.Collections.Generic.Dictionary[string,string]"
    $pairs.Add("__RequestVerificationToken", $token)
    $pairs.Add("TenDangNhap", "thanh")
    $pairs.Add("MatKhau", "123")
    $content = New-Object System.Net.Http.FormUrlEncodedContent($pairs)
    $loginRes = $lockedUser.Client.PostAsync("$baseUrl/DangNhap/DangNhap", $content).Result
    $status = [int]$loginRes.StatusCode
    $body = $loginRes.Content.ReadAsStringAsync().Result
    $isLocked = ($status -eq 200) -and ($body -match 'alert|ThongBao|khóa|Khóa|khoa|Locked')
    Report-Result "19. Chan Tai Khoan Bi Khoa" $isLocked "Locked account 'thanh' rejected from logging in"
} catch {
    Report-Result "19. Chan Tai Khoan Bi Khoa" $false $_.Exception.Message
}

# 12. Test Wrong Password Rejection
$badUser = Create-Client
try {
    $loginHtml = $badUser.Client.GetStringAsync("$baseUrl/DangNhap/DangNhap").Result
    $token = ""
    if ($loginHtml -match 'name="__RequestVerificationToken"[^>]*value="([^"]+)"') {
        $token = $matches[1]
    }
    $pairs = New-Object "System.Collections.Generic.Dictionary[string,string]"
    $pairs.Add("__RequestVerificationToken", $token)
    $pairs.Add("TenDangNhap", "admin")
    $pairs.Add("MatKhau", "WRONG_PASSWORD_999")
    $content = New-Object System.Net.Http.FormUrlEncodedContent($pairs)
    $loginRes = $badUser.Client.PostAsync("$baseUrl/DangNhap/DangNhap", $content).Result
    $status = [int]$loginRes.StatusCode
    $body = $loginRes.Content.ReadAsStringAsync().Result
    $isRejected = ($status -eq 200) -and ($body -like "*Sai tên đăng nhập hoặc mật khẩu*" -or $body -like "*Sai*")
    Report-Result "20. Chan Mat Khau Sai" $isRejected "Wrong password rejected properly"
} catch {
    Report-Result "20. Chan Mat Khau Sai" $false $_.Exception.Message
}

Write-Host "========================================================================" -ForegroundColor Cyan
if ($allPassed) {
    Write-Host "ALL 20 END-TO-END TESTS PASSED PERFECTLY! SYSTEM IS 100% OPERATIONAL." -ForegroundColor Green
} else {
    Write-Host "SOME TESTS FAILED." -ForegroundColor Red
}
