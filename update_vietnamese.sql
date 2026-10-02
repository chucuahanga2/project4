-- Cập nhật dữ liệu tiếng Việt chuẩn Unicode cho KhoaHocTrucTuyenDB
SET NOCOUNT ON;

-- 1. Cập nhật Tài khoản
UPDATE TaiKhoan SET TrangThai = N'Hoạt động' WHERE TrangThai LIKE '%Ho%t%d%ng%' OR TrangThai = 'Active' OR TrangThai IS NULL;
UPDATE TaiKhoan SET TrangThai = N'Khóa' WHERE TenDangNhap = 'thanh';

-- Cập nhật Giảng Viên
UPDATE GiangVien SET 
    TenGV = N'ThS. Huỳnh Phước', 
    ChuyenMon = N'Chuyên gia ASP.NET MVC, .NET Core, C# & SQL Server với hơn 8 năm kinh nghiệm giảng dạy và phát triển phần mềm doanh nghiệp.',
    BangCap = N'Thạc sĩ Khoa học Máy tính'
WHERE IDTenDangNhap = 'phuoc';

-- Cập nhật Học Viên
UPDATE HocVien SET 
    TenHV = N'Nguyễn Hoàng Phúc', 
    GioiTinh = N'Nam', 
    DiaChi = N'Quận 1, TP. Hồ Chí Minh'
WHERE IDTenDangNhap = 'phuc';

-- 2. Cập nhật Danh mục khóa học
DELETE FROM DanhMucKhoaHoc;
INSERT INTO DanhMucKhoaHoc (IDDanhMuc, TenDanhMuc, MoTa, TrangThai, ThuTu) VALUES
(1, N'Lập trình Web', N'Các khóa học xây dựng website từ cơ bản đến nâng cao với ASP.NET MVC, Frontend và Backend hiện đại.', N'Hoạt động', 1),
(2, N'Lập trình C# / .NET', N'Lộ trình làm chủ ngôn ngữ C#, kiến trúc hướng đối tượng OOP, LINQ và .NET Framework.', N'Hoạt động', 2),
(3, N'Cơ sở dữ liệu & SQL', N'Thiết kế, tối ưu truy vấn và quản trị cơ sở dữ liệu Microsoft SQL Server chuyên nghiệp.', N'Hoạt động', 3),
(4, N'Ngoại ngữ & Tiếng Anh', N'Khóa học Tiếng Anh giao tiếp, luyện thi chứng chỉ IELTS và TOEIC bài bản.', N'Hoạt động', 4),
(5, N'Kỹ năng Công nghệ', N'Kỹ năng lập trình thực chiến, cấu trúc dữ liệu, giải thuật và công cụ phần mềm.', N'Hoạt động', 5);

-- 3. Cập nhật Khóa học
UPDATE KhoaHoc SET 
    TenKhoaHoc = N'Lập trình Web với ASP.NET MVC 5 & Entity Framework 6',
    IDGiangVien = 100,
    IDDanhMuc = 1,
    DanhMuc = N'Lập trình Web',
    MoTa = N'Khóa học thực chiến toàn diện từ cơ bản đến chuyên sâu về kiến trúc ASP.NET MVC 5, Razor View, Entity Framework 6 Database First và tích hợp cổng thanh toán trực tuyến VNPay.',
    NoiDung = N'Khóa học này sẽ hướng dẫn bạn từng bước xây dựng một hệ thống website hoàn chỉnh từ sơ đồ cơ sở dữ liệu, mô hình MVC, Dependency Injection, bảo mật Session/Role đến tích hợp cổng thanh toán trực tuyến VNPay. Phù hợp cho sinh viên làm đồ án tốt nghiệp và lập trình viên muốn làm chủ công nghệ .NET.',
    HocPhi = 0,
    TrangThai = N'Hoạt động'
WHERE IDKhoaHoc = 'MVC2026';

IF NOT EXISTS (SELECT 1 FROM KhoaHoc WHERE IDKhoaHoc = 'CS2026')
BEGIN
    INSERT INTO KhoaHoc (IDKhoaHoc, TenKhoaHoc, IDGiangVien, IDDanhMuc, DanhMuc, MoTa, NoiDung, HinhAnhKH, HocPhi, TrangThai, NgayTao)
    VALUES ('CS2026', N'Lập Trình C# Toàn Diện Từ Zero Đến Hero', 100, 2, N'Lập trình C# / .NET',
    N'Khóa học nền tảng C# vững chắc, tư duy lập trình hướng đối tượng OOP, Collections, Generic, LINQ và xử lý đa luồng.',
    N'Lộ trình C# được thiết kế chuẩn quốc tế, giúp người học từ chưa biết gì đến tự tin viết các ứng dụng quản lý, ứng dụng doanh nghiệp với C# và .NET Framework.',
    'csharp.jpg', 499000, N'Hoạt động', GETDATE());
END
ELSE
BEGIN
    UPDATE KhoaHoc SET 
        TenKhoaHoc = N'Lập Trình C# Toàn Diện Từ Zero Đến Hero',
        IDGiangVien = 100,
        IDDanhMuc = 2,
        DanhMuc = N'Lập trình C# / .NET',
        MoTa = N'Khóa học nền tảng C# vững chắc, tư duy lập trình hướng đối tượng OOP, Collections, Generic, LINQ và xử lý đa luồng.',
        HocPhi = 499000,
        TrangThai = N'Hoạt động'
    WHERE IDKhoaHoc = 'CS2026';
END

IF NOT EXISTS (SELECT 1 FROM KhoaHoc WHERE IDKhoaHoc = 'SQL2026')
BEGIN
    INSERT INTO KhoaHoc (IDKhoaHoc, TenKhoaHoc, IDGiangVien, IDDanhMuc, DanhMuc, MoTa, NoiDung, HinhAnhKH, HocPhi, TrangThai, NgayTao)
    VALUES ('SQL2026', N'Làm Chủ Microsoft SQL Server & Tối Ưu Truy Vấn CSDL', 100, 3, N'Cơ sở dữ liệu & SQL',
    N'Khóa học thiết kế cơ sở dữ liệu chuẩn hóa, viết Store Procedure, Trigger, Transaction và kỹ thuật đánh Index tăng tốc.',
    N'Trang bị toàn bộ kỹ năng truy vấn nâng cao SQL, thiết kế lược đồ quan hệ chuẩn hóa 3NF, phân tích hiệu năng Execution Plan và tối ưu cơ sở dữ liệu thực tế.',
    'sqlserver.jpg', 299000, N'Hoạt động', GETDATE());
END
ELSE
BEGIN
    UPDATE KhoaHoc SET 
        TenKhoaHoc = N'Làm Chủ Microsoft SQL Server & Tối Ưu Truy Vấn CSDL',
        IDGiangVien = 100,
        IDDanhMuc = 3,
        DanhMuc = N'Cơ sở dữ liệu & SQL',
        MoTa = N'Khóa học thiết kế cơ sở dữ liệu chuẩn hóa, viết Store Procedure, Trigger, Transaction và kỹ thuật đánh Index tăng tốc.',
        HocPhi = 299000,
        TrangThai = N'Hoạt động'
    WHERE IDKhoaHoc = 'SQL2026';
END

IF NOT EXISTS (SELECT 1 FROM KhoaHoc WHERE IDKhoaHoc = 'WEBFE26')
BEGIN
    INSERT INTO KhoaHoc (IDKhoaHoc, TenKhoaHoc, IDGiangVien, IDDanhMuc, DanhMuc, MoTa, NoiDung, HinhAnhKH, HocPhi, TrangThai, NgayTao)
    VALUES ('WEBFE26', N'Lập Trình Frontend Hiện Đại HTML5, CSS3 & JavaScript', 100, 1, N'Lập trình Web',
    N'Tự tay xây dựng giao diện website tương tác mượt mà chuẩn responsive cho desktop, tablet và smartphone.',
    N'Khóa học dành cho người muốn xây dựng giao diện web đỉnh cao, làm chủ CSS Flexbox, Grid, hiệu ứng tương tác JavaScript ES6+ và tích hợp vào dự án MVC.',
    'frontend.jpg', 350000, N'Hoạt động', GETDATE());
END
ELSE
BEGIN
    UPDATE KhoaHoc SET 
        TenKhoaHoc = N'Lập Trình Frontend Hiện Đại HTML5, CSS3 & JavaScript',
        IDGiangVien = 100,
        IDDanhMuc = 1,
        DanhMuc = N'Lập trình Web',
        MoTa = N'Tự tay xây dựng giao diện website tương tác mượt mà chuẩn responsive cho desktop, tablet và smartphone.',
        HocPhi = 350000,
        TrangThai = N'Hoạt động'
    WHERE IDKhoaHoc = 'WEBFE26';
END

-- Cập nhật khóa học Tiếng Anh
UPDATE KhoaHoc SET 
    TenKhoaHoc = N'Luyện Thi IELTS Cấp Tốc - Mục Tiêu 6.5+ Toàn Diện',
    IDGiangVien = 100,
    IDDanhMuc = 4,
    DanhMuc = N'Ngoại ngữ & Tiếng Anh',
    MoTa = N'Chiến lược làm bài thi IELTS 4 kỹ năng Nghe - Nói - Đọc - Viết cùng bộ đề thi cập nhật mới nhất kèm lời giải chi tiết.',
    HocPhi = 1200000,
    TrangThai = N'Hoạt động'
WHERE IDKhoaHoc = 'TIA2025';

UPDATE KhoaHoc SET 
    TenKhoaHoc = N'Luyện Thi TOEIC Đột Phá 750+ Điểm Trong 60 Ngày',
    IDGiangVien = 100,
    IDDanhMuc = 4,
    DanhMuc = N'Ngoại ngữ & Tiếng Anh',
    MoTa = N'Tổng hợp mẹo tránh bẫy Part 1-7, mở rộng 1200 từ vựng cốt lõi và chiến thuật phân bổ thời gian phòng thi đỉnh cao.',
    HocPhi = 650000,
    TrangThai = N'Hoạt động'
WHERE IDKhoaHoc = 'TIB2025';

-- 4. Cập nhật Chương học và Bài học cho MVC2026
DELETE FROM TienDoHoc;
DELETE FROM BaiHoc;
DELETE FROM ChuongHoc;

-- Chương 1 của MVC2026
INSERT INTO ChuongHoc (IDChuong, IDKhoaHoc, TenChuong, MoTa, ThuTu)
VALUES (1, 'MVC2026', N'Chương 1: Tổng quan & Khởi tạo Dự án ASP.NET MVC 5', N'Làm quen với kiến trúc MVC, cài đặt Visual Studio và cấu trúc thư mục.', 1);

INSERT INTO BaiHoc (IDBaiHoc, IDChuong, TenBaiHoc, MoTa, NoiDung, VideoUrl, TaiLieuUrl, ThuTu, ThoiLuong, ChoXemThu)
VALUES 
(1, 1, N'Bài 1: Giới thiệu Kiến trúc ASP.NET MVC 5 & Luồng xử lý Request', 
 N'Hiểu rõ sự khác biệt giữa Web Forms và MVC, vai trò của Model, View, Controller.',
 N'Trong bài học này, chúng ta sẽ tìm hiểu về lịch sử ra đời của mô hình MVC (Model-View-Controller), cách ASP.NET MVC định tuyến các HTTP Request thông qua RouteConfig và gọi đến Action tương ứng trong Controller.',
 'https://www.youtube.com/embed/dQw4w9WgXcQ', 'https://learn.microsoft.com/aspnet/mvc', 1, 15, 1),

(2, 1, N'Bài 2: Khám phá Cấu trúc Project DuAnEnglish và Thư viện NuGet', 
 N'Chi tiết thư mục Controllers, Views, Models, App_Start và file Web.config cấu hình.',
 N'Hướng dẫn chi tiết cách tổ chức các thư mục, cấu hình kết nối SQL Server trong Web.config và quản lý các package Entity Framework, PagedList qua NuGet.',
 'https://www.youtube.com/embed/dQw4w9WgXcQ', 'https://learn.microsoft.com/aspnet/mvc/overview', 2, 22, 1),

(3, 1, N'Bài 3: Controller & Action: Nhận dữ liệu Request và Trả về Razor View', 
 N'Các phương thức ActionResult, ViewBag, ViewData, ViewModel và truyền dữ liệu.',
 N'Thực hành viết các Action GET/POST, truyền ViewModel từ Controller sang View và render dữ liệu với cú pháp Razor @model.',
 'https://www.youtube.com/embed/dQw4w9WgXcQ', '', 3, 28, 0);

-- Chương 2 của MVC2026
INSERT INTO ChuongHoc (IDChuong, IDKhoaHoc, TenChuong, MoTa, ThuTu)
VALUES (2, 'MVC2026', N'Chương 2: Entity Framework 6 & Thao tác Cơ sở dữ liệu', N'Làm chủ Database First với file edmx và viết truy vấn LINQ to Entities.', 2);

INSERT INTO BaiHoc (IDBaiHoc, IDChuong, TenBaiHoc, MoTa, NoiDung, VideoUrl, TaiLieuUrl, ThuTu, ThoiLuong, ChoXemThu)
VALUES 
(4, 2, N'Bài 4: Kết nối SQL Server với Entity Framework Database-First (.edmx)', 
 N'Tạo kết nối CSDL, sinh tự động DbContext và Entities từ các bảng SQL Server.',
 N'Hướng dẫn cách import bảng vào file edmx, cấu hình connection string và sử dụng DbContext để truy vấn dữ liệu hiệu quả.',
 'https://www.youtube.com/embed/dQw4w9WgXcQ', '', 1, 25, 0),

(5, 2, N'Bài 5: Viết truy vấn LINQ to Entities: Thêm, Sửa, Xóa và Tìm kiếm phân trang', 
 N'Kỹ thuật CRUD chuyên sâu, xử lý bất đồng bộ async/await và thư viện PagedList.Mvc.',
 N'Thực hành viết các câu lệnh LINQ Where, OrderBy, Include để nạp dữ liệu liên kết và hiển thị phân trang tối ưu.',
 'https://www.youtube.com/embed/dQw4w9WgXcQ', '', 2, 35, 0);

-- Chương 3 của MVC2026
INSERT INTO ChuongHoc (IDChuong, IDKhoaHoc, TenChuong, MoTa, ThuTu)
VALUES (3, 'MVC2026', N'Chương 3: Tích hợp Cổng thanh toán VNPay & Theo dõi Tiến độ', N'Tạo giao dịch thanh toán sandbox và tính toán tiến độ học tập tự động.', 3);

INSERT INTO BaiHoc (IDBaiHoc, IDChuong, TenBaiHoc, MoTa, NoiDung, VideoUrl, TaiLieuUrl, ThuTu, ThoiLuong, ChoXemThu)
VALUES 
(6, 3, N'Bài 6: Xây dựng Luồng Đăng ký & Tích hợp Cổng thanh toán VNPay Sandbox', 
 N'Thuật toán tạo chữ ký SHA512, tạo URL thanh toán và xử lý IPN/ReturnURL.',
 N'Quy trình bảo mật giao dịch, kiểm tra mã phản hồi vnp_ResponseCode = 00 và tự động kích hoạt trạng thái truy cập khóa học cho học viên.',
 'https://www.youtube.com/embed/dQw4w9WgXcQ', 'https://sandbox.vnpayment.vn/apis/docs/huong-dan-tich-hop/', 1, 40, 0),

(7, 3, N'Bài 7: Xây dựng Giao diện Học Trực Tuyến & Lưu Tiến độ AJAX', 
 N'Thiết kế video player chuẩn Tiki/Coursera, ghi nhận bài học đã hoàn thành.',
 N'Lập trình AJAX gửi yêu cầu đánh dấu hoàn thành bài học, cập nhật % tiến độ hiển thị trực quan và lưu vết vào bảng TienDoHoc.',
 'https://www.youtube.com/embed/dQw4w9WgXcQ', '', 2, 30, 0);

-- Thêm chương & bài học mẫu cho CS2026
INSERT INTO ChuongHoc (IDChuong, IDKhoaHoc, TenChuong, MoTa, ThuTu)
VALUES (4, 'CS2026', N'Chương 1: Cú pháp Cơ bản & Kiểu dữ liệu trong C#', N'Khai báo biến, kiểu dữ liệu, cấu trúc điều khiển if-else và vòng lặp.', 1);

INSERT INTO BaiHoc (IDBaiHoc, IDChuong, TenBaiHoc, MoTa, NoiDung, VideoUrl, TaiLieuUrl, ThuTu, ThoiLuong, ChoXemThu)
VALUES 
(8, 4, N'Bài 1: Cài đặt .NET SDK & Viết chương trình Hello World', 
 N'Khởi tạo Console Application đầu tiên với C#.',
 N'Hướng dẫn cài đặt môi trường lập trình C#, hiểu cấu trúc hàm Main() và câu lệnh Console.WriteLine().',
 'https://www.youtube.com/embed/dQw4w9WgXcQ', '', 1, 12, 1),
(9, 4, N'Bài 2: Kiểu dữ liệu nguyên thủy, Ép kiểu & Toán tử', 
 N'Phân biệt Value types và Reference types trong C#.',
 N'Tìm hiểu int, double, string, boolean và các phép toán logic, ép kiểu ngầm định và tường minh.',
 'https://www.youtube.com/embed/dQw4w9WgXcQ', '', 2, 20, 1);

-- 5. Cập nhật Đăng Ký Khóa Học cho học viên mẫu 'phuc' (IDHocVien = 2500)
DELETE FROM DangKyKhoaHoc WHERE IDHocVien = 2500;
INSERT INTO DangKyKhoaHoc (IDHocVien, IDKhoaHoc, NgayDangKy, TrangThai, NgayHoanThanh)
VALUES 
(2500, 'MVC2026', GETDATE(), N'Đã kích hoạt', NULL),
(2500, 'CS2026', GETDATE(), N'Đã kích hoạt', NULL);

-- Đánh dấu hoàn thành bài 1 và bài 2 cho học viên 2500
INSERT INTO TienDoHoc (IDHocVien, IDBaiHoc, DaHoanThanh, NgayHoanThanh, ThoiDiemXemGanNhat)
VALUES 
(2500, 1, 1, GETDATE(), GETDATE()),
(2500, 2, 1, GETDATE(), GETDATE());

PRINT N'CẬP NHẬT DỮ LIỆU TIẾNG VIỆT UNICODE THÀNH CÔNG!';
