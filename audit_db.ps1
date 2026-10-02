$connString = "Server=localhost;Database=KhoaHocTrucTuyenDB;Integrated Security=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()

Write-Host "=== COLUMNS IN TaiKhoan ==="
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='TaiKhoan'"
$r = $cmd.ExecuteReader()
while ($r.Read()) {
    Write-Host "$($r[0]) | $($r[1]) | $($r[2])"
}
$r.Close()

Write-Host "`n=== ACCOUNTS IN TaiKhoan ==="
$cmd.CommandText = "SELECT TenDangNhap, LoaiTK, MatKhau, TrangThai FROM TaiKhoan"
$r = $cmd.ExecuteReader()
while ($r.Read()) {
    Write-Host "$($r[0]) | $($r[1]) | $($r[2]) | $($r[3])"
}
$r.Close()

Write-Host "`n=== COURSES IN KhoaHoc ==="
$cmd.CommandText = "SELECT IDKhoaHoc, TenKhoaHoc, HocPhi, TrangThai, IDDanhMuc, IDGiangVien FROM KhoaHoc"
$r = $cmd.ExecuteReader()
while ($r.Read()) {
    Write-Host "$($r[0]) | $($r[1]) | $($r[2]) | $($r[3]) | $($r[4]) | $($r[5])"
}
$r.Close()

Write-Host "`n=== CATEGORIES IN DanhMucKhoaHoc ==="
$cmd.CommandText = "SELECT IDDanhMuc, TenDanhMuc, TrangThai, ThuTu FROM DanhMucKhoaHoc"
$r = $cmd.ExecuteReader()
while ($r.Read()) {
    Write-Host "$($r[0]) | $($r[1]) | $($r[2]) | $($r[3])"
}
$r.Close()

$conn.Close()
