-- ==============================================================================
-- SCRIPT CẬP NHẬT CƠ SỞ DỮ LIỆU: XÂY DỰNG WEBSITE CHIA SẺ KHÓA HỌC TRỰC TUYẾN
-- Database: KhoaHocTrucTuyenDB
-- ==============================================================================
USE KhoaHocTrucTuyenDB;
GO

-- 1. TẠO BẢNG DanhMucKhoaHoc
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DanhMucKhoaHoc')
BEGIN
    CREATE TABLE DanhMucKhoaHoc (
        IDDanhMuc INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        TenDanhMuc NVARCHAR(150) NOT NULL,
        MoTa NVARCHAR(MAX) NULL,
        TrangThai NVARCHAR(50) DEFAULT N'Hoạt động',
        ThuTu INT DEFAULT 0
    );
    PRINT N'Đã tạo bảng DanhMucKhoaHoc thành công.';
END
GO

-- Chèn danh mục mặc định phong phú (Lập trình, Database, Ngoại ngữ...)
IF NOT EXISTS (SELECT * FROM DanhMucKhoaHoc WHERE TenDanhMuc = N'Lập trình Web')
BEGIN
    INSERT INTO DanhMucKhoaHoc (TenDanhMuc, MoTa, TrangThai, ThuTu) VALUES
    (N'Lập trình Web', N'Các khóa học HTML, CSS, JavaScript, ASP.NET MVC, React, Vue, v.v.', N'Hoạt động', 1),
    (N'Lập trình C# / .NET', N'Ngôn ngữ C#, .NET Framework, .NET Core, WinForms, WPF, Web API', N'Hoạt động', 2),
    (N'Cơ sở dữ liệu', N'SQL Server, MySQL, PostgreSQL, thiết kế CSDL và tối ưu truy vấn', N'Hoạt động', 3),
    (N'Tiếng Anh & Ngoại ngữ', N'Khóa học tiếng Anh giao tiếp, luyện thi chứng chỉ TOEIC, IELTS', N'Hoạt động', 4),
    (N'Kỹ năng Công nghệ', N'Git, DevOps, Docker, Agile Scrum và kỹ năng mềm ngành CNTT', N'Hoạt động', 5);
    PRINT N'Đã chèn dữ liệu mẫu vào DanhMucKhoaHoc.';
END
GO

-- 2. CẬP NHẬT BẢNG KhoaHoc: Mở rộng các trường cần thiết cho khóa học trực tuyến
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoaHoc') AND name = 'IDDanhMuc')
BEGIN
    ALTER TABLE KhoaHoc ADD IDDanhMuc INT NULL;
    ALTER TABLE KhoaHoc ADD CONSTRAINT FK_KhoaHoc_DanhMuc FOREIGN KEY (IDDanhMuc) REFERENCES DanhMucKhoaHoc(IDDanhMuc);
    PRINT N'Đã thêm cột IDDanhMuc vào KhoaHoc.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoaHoc') AND name = 'IDGiangVien')
BEGIN
    ALTER TABLE KhoaHoc ADD IDGiangVien INT NULL;
    ALTER TABLE KhoaHoc ADD CONSTRAINT FK_KhoaHoc_GiangVien FOREIGN KEY (IDGiangVien) REFERENCES GiangVien(IDGiangVien);
    PRINT N'Đã thêm cột IDGiangVien vào KhoaHoc.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoaHoc') AND name = 'NoiDung')
BEGIN
    ALTER TABLE KhoaHoc ADD NoiDung NVARCHAR(MAX) NULL;
    PRINT N'Đã thêm cột NoiDung vào KhoaHoc.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoaHoc') AND name = 'TrangThai')
BEGIN
    ALTER TABLE KhoaHoc ADD TrangThai NVARCHAR(50) DEFAULT N'Hiển thị';
    PRINT N'Đã thêm cột TrangThai vào KhoaHoc.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoaHoc') AND name = 'NgayTao')
BEGIN
    ALTER TABLE KhoaHoc ADD NgayTao DATETIME DEFAULT GETDATE();
    PRINT N'Đã thêm cột NgayTao vào KhoaHoc.';
END
GO

-- Cập nhật dữ liệu cũ nếu TrangThai hoặc NgayTao bị NULL
UPDATE KhoaHoc SET TrangThai = N'Hiển thị' WHERE TrangThai IS NULL;
UPDATE KhoaHoc SET NgayTao = GETDATE() WHERE NgayTao IS NULL;
-- Gán IDDanhMuc ngoại ngữ cho các khóa học cũ nếu chưa có
DECLARE @idDM_NgoaiNgu INT;
SELECT TOP 1 @idDM_NgoaiNgu = IDDanhMuc FROM DanhMucKhoaHoc WHERE TenDanhMuc LIKE N'%Ngoại ngữ%' OR TenDanhMuc LIKE N'%Tiếng Anh%';
IF (@idDM_NgoaiNgu IS NULL)
    SELECT TOP 1 @idDM_NgoaiNgu = IDDanhMuc FROM DanhMucKhoaHoc;
IF (@idDM_NgoaiNgu IS NOT NULL)
    UPDATE KhoaHoc SET IDDanhMuc = @idDM_NgoaiNgu WHERE IDDanhMuc IS NULL;

-- Gán giảng viên mặc định cho các khóa học cũ
DECLARE @firstGV INT;
SELECT TOP 1 @firstGV = IDGiangVien FROM GiangVien;
IF (@firstGV IS NOT NULL)
BEGIN
    UPDATE KhoaHoc SET IDGiangVien = @firstGV WHERE IDGiangVien IS NULL;
END
GO

-- Thêm các khóa học lập trình tiêu biểu vào KhoaHoc
IF NOT EXISTS (SELECT * FROM KhoaHoc WHERE IDKhoaHoc = 'MVC2026')
BEGIN
    INSERT INTO KhoaHoc (IDKhoaHoc, TenKhoaHoc, IDDanhMuc, IDGiangVien, DanhMuc, MoTa, NoiDung, HocPhi, HinhAnhKH, TrangThai, NgayTao)
    VALUES (
        'MVC2026', 
        N'Lập trình Web với ASP.NET MVC 5 & Entity Framework', 
        1, 
        (SELECT TOP 1 IDGiangVien FROM GiangVien),
        N'Lập trình Web', 
        N'Khóa học từ cơ bản đến nâng cao về kiến trúc ASP.NET MVC 5, Razor, Entity Framework 6 Database First và tích hợp cổng thanh toán VNPay.', 
        N'<h3>Mục tiêu khóa học:</h3><ul><li>Nắm vững mô hình MVC (Model - View - Controller).</li><li>Làm chủ Entity Framework 6 và SQL Server.</li><li>Xây dựng ứng dụng web thương mại điện tử / học trực tuyến hoàn chỉnh.</li><li>Tích hợp thanh toán trực tuyến qua cổng VNPay.</li></ul>', 
        499000, 
        N'mvc_banner.png', 
        N'Hiển thị', 
        GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM KhoaHoc WHERE IDKhoaHoc = 'CSHARP01')
BEGIN
    INSERT INTO KhoaHoc (IDKhoaHoc, TenKhoaHoc, IDDanhMuc, IDGiangVien, DanhMuc, MoTa, NoiDung, HocPhi, HinhAnhKH, TrangThai, NgayTao)
    VALUES (
        'CSHARP01', 
        N'Lập trình C# Cơ Bản đến Nâng Cao cho người mới bắt đầu', 
        2, 
        (SELECT TOP 1 IDGiangVien FROM GiangVien),
        N'Lập trình C# / .NET', 
        N'Nền tảng lập trình C#, tư duy lập trình hướng đối tượng OOP, Generic, LINQ và xử lý ngoại lệ trong C#.', 
        N'<h3>Nội dung khóa học:</h3><p>Học lập trình C# bài bản, cấu trúc điều khiển, mảng, OOP, class, interface, LINQ to Objects.</p>', 
        0, -- Khóa học miễn phí
        N'csharp_banner.png', 
        N'Hiển thị', 
        GETDATE()
    );
END
GO

-- 3. TẠO BẢNG ChuongHoc
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChuongHoc')
BEGIN
    CREATE TABLE ChuongHoc (
        IDChuong INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        IDKhoaHoc NVARCHAR(50) NOT NULL,
        TenChuong NVARCHAR(255) NOT NULL,
        MoTa NVARCHAR(MAX) NULL,
        ThuTu INT DEFAULT 1,
        CONSTRAINT FK_ChuongHoc_KhoaHoc FOREIGN KEY (IDKhoaHoc) REFERENCES KhoaHoc(IDKhoaHoc) ON DELETE CASCADE
    );
    PRINT N'Đã tạo bảng ChuongHoc thành công.';
END
GO

-- 4. TẠO BẢNG BaiHoc
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BaiHoc')
BEGIN
    CREATE TABLE BaiHoc (
        IDBaiHoc INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        IDChuong INT NOT NULL,
        TenBaiHoc NVARCHAR(255) NOT NULL,
        MoTa NVARCHAR(MAX) NULL,
        NoiDung NVARCHAR(MAX) NULL,
        VideoUrl NVARCHAR(500) NULL,
        TaiLieuUrl NVARCHAR(500) NULL,
        ThuTu INT DEFAULT 1,
        ThoiLuong INT DEFAULT 0,
        ChoXemThu BIT DEFAULT 0,
        CONSTRAINT FK_BaiHoc_ChuongHoc FOREIGN KEY (IDChuong) REFERENCES ChuongHoc(IDChuong) ON DELETE CASCADE
    );
    PRINT N'Đã tạo bảng BaiHoc thành công.';
END
GO

-- Chèn dữ liệu mẫu cho Chương học và Bài học khóa MVC2026
IF EXISTS (SELECT * FROM KhoaHoc WHERE IDKhoaHoc = 'MVC2026') 
   AND NOT EXISTS (SELECT * FROM ChuongHoc WHERE IDKhoaHoc = 'MVC2026')
BEGIN
    -- Chương 1
    INSERT INTO ChuongHoc (IDKhoaHoc, TenChuong, MoTa, ThuTu) 
    VALUES ('MVC2026', N'Chương 1: Tổng quan và Thiết lập Môi trường ASP.NET MVC', N'Giới thiệu kiến trúc MVC, cài đặt Visual Studio và khởi tạo project đầu tiên', 1);
    DECLARE @C1 INT = SCOPE_IDENTITY();

    INSERT INTO BaiHoc (IDChuong, TenBaiHoc, MoTa, NoiDung, VideoUrl, TaiLieuUrl, ThuTu, ThoiLuong, ChoXemThu) VALUES
    (@C1, N'Bài 1: Giới thiệu mô hình MVC và các thành phần cốt lõi', N'Tìm hiểu Model, View, Controller tương tác với nhau như thế nào', N'<p>Mô hình Model-View-Controller giúp phân tách rõ ràng giao diện, logic xử lý và dữ liệu.</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'/Content/docs/bai1_gioithieu_mvc.pdf', 1, 15, 1),
    (@C1, N'Bài 2: Cấu trúc thư mục dự án ASP.NET MVC 5', N'Ý nghĩa các thư mục Controllers, Views, Models, App_Start...', N'<p>Thư mục App_Start chứa RouteConfig để định tuyến URL cho ứng dụng web.</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'/Content/docs/bai2_cautruc_project.pdf', 2, 20, 1),
    (@C1, N'Bài 3: Controller và Action: Xử lý Request và Return View', N'Cách tạo Controller, định nghĩa Action và truyền dữ liệu ra View với ViewBag, ViewData', N'<p>Mỗi action method trả về một ActionResult (thường là ViewResult).</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'/Content/docs/bai3_controller_action.pdf', 3, 25, 0);

    -- Chương 2
    INSERT INTO ChuongHoc (IDKhoaHoc, TenChuong, MoTa, ThuTu) 
    VALUES ('MVC2026', N'Chương 2: Model và Cơ sở dữ liệu Entity Framework 6', N'Làm việc với SQL Server và Entity Framework theo hướng Database First', 2);
    DECLARE @C2 INT = SCOPE_IDENTITY();

    INSERT INTO BaiHoc (IDChuong, TenBaiHoc, MoTa, NoiDung, VideoUrl, TaiLieuUrl, ThuTu, ThoiLuong, ChoXemThu) VALUES
    (@C2, N'Bài 1: Giới thiệu Entity Framework 6 & Hướng tiếp cận Database First', N'Kết nối CSDL SQL Server và sinh file edmx tự động', N'<p>Entity Framework Database First giúp tự động mapping bảng trong CSDL thành các C# class.</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'/Content/docs/bai4_ef6_database_first.pdf', 1, 30, 0),
    (@C2, N'Bài 2: Thao tác CRUD hoàn chỉnh với DbContext và LINQ', N'Thêm, sửa, xóa, tìm kiếm dữ liệu an toàn với Entity Framework', N'<p>Sử dụng db.KhoaHocs.Add(), db.SaveChanges(), Find() và FirstOrDefault().</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'/Content/docs/bai5_crud_linq.pdf', 2, 35, 0);
END
GO

-- 5. TẠO BẢNG DangKyKhoaHoc
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DangKyKhoaHoc')
BEGIN
    CREATE TABLE DangKyKhoaHoc (
        IDDangKy INT IDENTITY(1000,1) PRIMARY KEY NOT NULL,
        IDHocVien INT NOT NULL,
        IDKhoaHoc NVARCHAR(50) NOT NULL,
        NgayDangKy DATETIME DEFAULT GETDATE(),
        TrangThai NVARCHAR(50) DEFAULT N'Chưa kích hoạt',
        NgayHoanThanh DATETIME NULL,
        CONSTRAINT FK_DangKy_HocVien FOREIGN KEY (IDHocVien) REFERENCES HocVien(IDHocVien),
        CONSTRAINT FK_DangKy_KhoaHoc FOREIGN KEY (IDKhoaHoc) REFERENCES KhoaHoc(IDKhoaHoc),
        CONSTRAINT UQ_HocVien_KhoaHoc UNIQUE (IDHocVien, IDKhoaHoc)
    );
    PRINT N'Đã tạo bảng DangKyKhoaHoc thành công.';
END
GO

-- 6. TẠO BẢNG TienDoHoc
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TienDoHoc')
BEGIN
    CREATE TABLE TienDoHoc (
        IDTienDo INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        IDHocVien INT NOT NULL,
        IDBaiHoc INT NOT NULL,
        DaHoanThanh BIT DEFAULT 0,
        NgayHoanThanh DATETIME NULL,
        ThoiDiemXemGanNhat DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_TienDo_HocVien FOREIGN KEY (IDHocVien) REFERENCES HocVien(IDHocVien),
        CONSTRAINT FK_TienDo_BaiHoc FOREIGN KEY (IDBaiHoc) REFERENCES BaiHoc(IDBaiHoc) ON DELETE CASCADE,
        CONSTRAINT UQ_HocVien_BaiHoc UNIQUE (IDHocVien, IDBaiHoc)
    );
    PRINT N'Đã tạo bảng TienDoHoc thành công.';
END
GO

-- 7. CẬP NHẬT BẢNG ThanhToan: Hỗ trợ thanh toán theo khóa học online
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ThanhToan') AND name = 'IDLopHoc')
BEGIN
    -- Cho phép IDLopHoc NULL để hóa đơn khóa học trực tuyến không bắt buộc phải có lớp học vật lý
    ALTER TABLE ThanhToan ALTER COLUMN IDLopHoc NVARCHAR(50) NULL;
    PRINT N'Đã cập nhật IDLopHoc trong ThanhToan sang NULLABLE.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ThanhToan') AND name = 'IDDangKy')
BEGIN
    ALTER TABLE ThanhToan ADD IDDangKy INT NULL;
    ALTER TABLE ThanhToan ADD CONSTRAINT FK_ThanhToan_DangKy FOREIGN KEY (IDDangKy) REFERENCES DangKyKhoaHoc(IDDangKy);
    PRINT N'Đã thêm cột IDDangKy vào ThanhToan.';
END
GO

PRINT N'==============================================================================';
PRINT N'CẬP NHẬT DATABASE THÀNH CÔNG!';
PRINT N'==============================================================================';
