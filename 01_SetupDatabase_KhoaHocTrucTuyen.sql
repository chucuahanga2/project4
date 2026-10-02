-- ==============================================================================
-- SCRIPT THIẾT LẬP VÀ KHỞI TẠO CƠ SỞ DỮ LIỆU ĐẦY ĐỦ: WEBSITE CHIA SẺ KHÓA HỌC TRỰC TUYẾN
-- Tên Database duy nhất: KhoaHocTrucTuyenDB
-- Hướng tiếp cận: Entity Framework 6 Database First
-- Tính chất: All-in-one, Self-contained, Idempotent (Chạy một lần là có đầy đủ toàn bộ bảng và dữ liệu mẫu)
-- ==============================================================================

-- 1. TẠO CƠ SỞ DỮ LIỆU NẾU CHƯA CÓ
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'KhoaHocTrucTuyenDB')
BEGIN
    CREATE DATABASE KhoaHocTrucTuyenDB;
    PRINT N'[OK] Đã tạo cơ sở dữ liệu KhoaHocTrucTuyenDB thành công.';
END
ELSE
BEGIN
    PRINT N'[INFO] Database KhoaHocTrucTuyenDB đã tồn tại.';
END
GO

USE KhoaHocTrucTuyenDB;
GO

SET NOCOUNT ON;

-- ==============================================================================
-- 2. TẠO CÁC BẢNG CỐT LÕI (SCHEMA DEFINITION)
-- ==============================================================================

-- 2.1 Bảng LoaiTaiKhoan (Phân quyền người dùng)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LoaiTaiKhoan')
BEGIN
    CREATE TABLE LoaiTaiKhoan (
        LoaiTK NVARCHAR(20) PRIMARY KEY NOT NULL
    );
    PRINT N'[OK] Đã tạo bảng LoaiTaiKhoan.';
END
GO

-- 2.2 Bảng TaiKhoan (Quản lý tài khoản đăng nhập)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TaiKhoan')
BEGIN
    CREATE TABLE TaiKhoan (
        TenDangNhap VARCHAR(50) PRIMARY KEY NOT NULL,
        LoaiTK NVARCHAR(20) NULL,
        MatKhau VARCHAR(256) NOT NULL,
        Email VARCHAR(100) NULL,
        SDT VARCHAR(15) NULL,
        TrangThai NVARCHAR(50) DEFAULT N'Hoạt động',
        CONSTRAINT FK_TaiKhoan_LoaiTK FOREIGN KEY (LoaiTK) REFERENCES LoaiTaiKhoan(LoaiTK)
    );
    PRINT N'[OK] Đã tạo bảng TaiKhoan.';
END
ELSE
BEGIN
    -- Mở rộng độ dài cột MatKhau nếu đang là phiên bản cũ (< 256 ký tự)
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TaiKhoan' AND COLUMN_NAME = 'MatKhau' AND CHARACTER_MAXIMUM_LENGTH < 256)
    BEGIN
        ALTER TABLE TaiKhoan ALTER COLUMN MatKhau VARCHAR(256) NOT NULL;
        PRINT N'[OK] Đã mở rộng TaiKhoan.MatKhau lên VARCHAR(256).';
    END
END
GO

-- 2.3 Bảng HocVien (Hồ sơ học viên)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HocVien')
BEGIN
    CREATE TABLE HocVien (
        IDHocVien INT IDENTITY(2500,1) PRIMARY KEY NOT NULL,
        IDTenDangNhap VARCHAR(50) NULL,
        TenHV NVARCHAR(100) NULL,
        NgaySinh DATE NULL,
        GioiTinh NVARCHAR(10) NULL,
        DiaChi NVARCHAR(255) NULL,
        CONSTRAINT FK_HocVien_TaiKhoan FOREIGN KEY (IDTenDangNhap) REFERENCES TaiKhoan(TenDangNhap) ON DELETE CASCADE
    );
    PRINT N'[OK] Đã tạo bảng HocVien.';
END
GO

-- 2.4 Bảng GiangVien (Hồ sơ giảng viên)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GiangVien')
BEGIN
    CREATE TABLE GiangVien (
        IDGiangVien INT IDENTITY(100,1) PRIMARY KEY NOT NULL,
        IDTenDangNhap VARCHAR(50) NULL,
        TenGV NVARCHAR(100) NULL,
        NgaySinh DATE NULL,
        GioiTinh NVARCHAR(10) NULL,
        DiaChi NVARCHAR(255) NULL,
        ChuyenMon NVARCHAR(MAX) NULL,
        BangCap NVARCHAR(MAX) NULL,
        CONSTRAINT FK_GiangVien_TaiKhoan FOREIGN KEY (IDTenDangNhap) REFERENCES TaiKhoan(TenDangNhap) ON DELETE CASCADE
    );
    PRINT N'[OK] Đã tạo bảng GiangVien.';
END
GO

-- 2.5 Bảng DanhMucKhoaHoc (Phân loại danh mục khóa học trực tuyến)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DanhMucKhoaHoc')
BEGIN
    CREATE TABLE DanhMucKhoaHoc (
        IDDanhMuc INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        TenDanhMuc NVARCHAR(150) NOT NULL,
        MoTa NVARCHAR(MAX) NULL,
        TrangThai NVARCHAR(50) DEFAULT N'Hoạt động',
        ThuTu INT DEFAULT 0
    );
    PRINT N'[OK] Đã tạo bảng DanhMucKhoaHoc.';
END
GO

-- 2.6 Bảng KhoaHoc (Thông tin khóa học)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'KhoaHoc')
BEGIN
    CREATE TABLE KhoaHoc (
        IDKhoaHoc NVARCHAR(50) PRIMARY KEY NOT NULL,
        TenKhoaHoc NVARCHAR(255) NOT NULL,
        DanhMuc NVARCHAR(50) NULL,
        IDDanhMuc INT NULL,
        IDGiangVien INT NULL,
        MoTa NVARCHAR(MAX) NULL,
        NoiDung NVARCHAR(MAX) NULL,
        HocPhi DECIMAL(18, 0) DEFAULT 0,
        HinhAnhKH NVARCHAR(50) NULL,
        TrangThai NVARCHAR(50) DEFAULT N'Hiển thị',
        NgayTao DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_KhoaHoc_DanhMuc FOREIGN KEY (IDDanhMuc) REFERENCES DanhMucKhoaHoc(IDDanhMuc),
        CONSTRAINT FK_KhoaHoc_GiangVien FOREIGN KEY (IDGiangVien) REFERENCES GiangVien(IDGiangVien)
    );
    PRINT N'[OK] Đã tạo bảng KhoaHoc.';
END
ELSE
BEGIN
    -- Mở rộng cột nếu thiếu các trường phục vụ khóa học trực tuyến
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoaHoc') AND name = 'IDDanhMuc')
    BEGIN
        ALTER TABLE KhoaHoc ADD IDDanhMuc INT NULL;
        ALTER TABLE KhoaHoc ADD CONSTRAINT FK_KhoaHoc_DanhMuc FOREIGN KEY (IDDanhMuc) REFERENCES DanhMucKhoaHoc(IDDanhMuc);
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoaHoc') AND name = 'IDGiangVien')
    BEGIN
        ALTER TABLE KhoaHoc ADD IDGiangVien INT NULL;
        ALTER TABLE KhoaHoc ADD CONSTRAINT FK_KhoaHoc_GiangVien FOREIGN KEY (IDGiangVien) REFERENCES GiangVien(IDGiangVien);
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoaHoc') AND name = 'NoiDung')
    BEGIN
        ALTER TABLE KhoaHoc ADD NoiDung NVARCHAR(MAX) NULL;
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoaHoc') AND name = 'TrangThai')
    BEGIN
        ALTER TABLE KhoaHoc ADD TrangThai NVARCHAR(50) DEFAULT N'Hiển thị';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KhoaHoc') AND name = 'NgayTao')
    BEGIN
        ALTER TABLE KhoaHoc ADD NgayTao DATETIME DEFAULT GETDATE();
    END
END
GO

-- 2.7 Bảng ChuongHoc (Chương mục khóa học trực tuyến)
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
    PRINT N'[OK] Đã tạo bảng ChuongHoc.';
END
GO

-- 2.8 Bảng BaiHoc (Bài học, Video bài giảng, Tài liệu và Học thử)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BaiHoc')
BEGIN
    CREATE TABLE BaiHoc (
        IDBaiHoc INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        IDChuong INT NOT NULL,
        TenBaiHoc NVARCHAR(250) NOT NULL,
        MoTa NVARCHAR(MAX) NULL,
        NoiDung NVARCHAR(MAX) NULL,
        VideoUrl NVARCHAR(500) NULL,
        TaiLieuUrl NVARCHAR(500) NULL,
        ThuTu INT DEFAULT 1,
        ThoiLuong INT DEFAULT 0,
        ChoXemThu BIT DEFAULT 0,
        CONSTRAINT FK_BaiHoc_ChuongHoc FOREIGN KEY (IDChuong) REFERENCES ChuongHoc(IDChuong) ON DELETE CASCADE
    );
    PRINT N'[OK] Đã tạo bảng BaiHoc.';
END
GO

-- 2.9 Bảng DangKyKhoaHoc (Ghi danh và Kích hoạt khóa học trực tuyến)
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
    PRINT N'[OK] Đã tạo bảng DangKyKhoaHoc.';
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_HocVien_KhoaHoc' AND object_id = OBJECT_ID('DangKyKhoaHoc'))
    BEGIN
        ALTER TABLE DangKyKhoaHoc ADD CONSTRAINT UQ_HocVien_KhoaHoc UNIQUE (IDHocVien, IDKhoaHoc);
    END
END
GO

-- 2.10 Bảng TienDoHoc (Theo dõi tiến độ hoàn thành từng bài học)
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
    PRINT N'[OK] Đã tạo bảng TienDoHoc.';
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_HocVien_BaiHoc' AND object_id = OBJECT_ID('TienDoHoc'))
    BEGIN
        ALTER TABLE TienDoHoc ADD CONSTRAINT UQ_HocVien_BaiHoc UNIQUE (IDHocVien, IDBaiHoc);
    END
END
GO

-- 2.11 Bảng PhongHoc (Hỗ trợ module lớp học trực tiếp/phòng học cũ)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PhongHoc')
BEGIN
    CREATE TABLE PhongHoc (
        IDPhongHoc INT PRIMARY KEY NOT NULL,
        TenPhong NVARCHAR(50) NULL,
        SucChua INT NULL
    );
    PRINT N'[OK] Đã tạo bảng PhongHoc.';
END
GO

-- 2.12 Bảng LopHoc (Quản lý lớp học truyền thống)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LopHoc')
BEGIN
    CREATE TABLE LopHoc (
        IDLopHoc NVARCHAR(50) PRIMARY KEY NOT NULL,
        IDPhongHoc INT NULL,
        IDKhoaHoc NVARCHAR(50) NULL,
        IDGiangVien INT NULL,
        TenLop VARCHAR(100) NULL,
        Slot INT NULL,
        ThuTrongTuan NVARCHAR(50) NULL,
        GioHocBD TIME NULL,
        GioHocKT TIME NULL,
        CONSTRAINT FK_LopHoc_KhoaHoc FOREIGN KEY (IDKhoaHoc) REFERENCES KhoaHoc(IDKhoaHoc),
        CONSTRAINT FK_LopHoc_GiangVien FOREIGN KEY (IDGiangVien) REFERENCES GiangVien(IDGiangVien),
        CONSTRAINT FK_LopHoc_PhongHoc FOREIGN KEY (IDPhongHoc) REFERENCES PhongHoc(IDPhongHoc)
    );
    PRINT N'[OK] Đã tạo bảng LopHoc.';
END
GO

-- 2.13 Bảng ThanhToan (Hóa đơn và Lịch sử thanh toán học phí)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ThanhToan')
BEGIN
    CREATE TABLE ThanhToan (
        IDThanhToan INT IDENTITY(202500,1) PRIMARY KEY NOT NULL,
        IDLopHoc NVARCHAR(50) NULL,
        TenDangNhap VARCHAR(50) NULL,
        IDKhoaHoc NVARCHAR(50) NULL,
        IDDangKy INT NULL,
        SoTien DECIMAL(18, 0) NULL,
        PhuongThucTT NVARCHAR(50) NULL,
        NgayThanhToan DATETIME NULL,
        TrangThai NVARCHAR(50) DEFAULT N'Chưa thanh toán',
        NgayXacNhan DATETIME NULL,
        CONSTRAINT FK_ThanhToan_LopHoc FOREIGN KEY (IDLopHoc) REFERENCES LopHoc(IDLopHoc),
        CONSTRAINT FK_ThanhToan_TaiKhoan FOREIGN KEY (TenDangNhap) REFERENCES TaiKhoan(TenDangNhap),
        CONSTRAINT FK_ThanhToan_KhoaHoc FOREIGN KEY (IDKhoaHoc) REFERENCES KhoaHoc(IDKhoaHoc),
        CONSTRAINT FK_ThanhToan_DangKy FOREIGN KEY (IDDangKy) REFERENCES DangKyKhoaHoc(IDDangKy)
    );
    PRINT N'[OK] Đã tạo bảng ThanhToan.';
END
ELSE
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ThanhToan') AND name = 'IDLopHoc')
    BEGIN
        ALTER TABLE ThanhToan ALTER COLUMN IDLopHoc NVARCHAR(50) NULL;
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ThanhToan') AND name = 'IDDangKy')
    BEGIN
        ALTER TABLE ThanhToan ADD IDDangKy INT NULL;
        ALTER TABLE ThanhToan ADD CONSTRAINT FK_ThanhToan_DangKy FOREIGN KEY (IDDangKy) REFERENCES DangKyKhoaHoc(IDDangKy);
    END
END
GO

-- 2.14 Bảng GiaoDichVNPAY (Nhật ký giao dịch cổng thanh toán VNPay)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GiaoDichVNPAY')
BEGIN
    CREATE TABLE GiaoDichVNPAY (
        IDGiaoDich INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        IDThanhToan INT NULL,
        MaGiaoDich VARCHAR(100) NULL,
        NgayGiaoDich DATETIME DEFAULT GETDATE(),
        SoTien DECIMAL(18, 0) NULL,
        TrangThai NVARCHAR(50) NULL,
        NoiDung NVARCHAR(MAX) NULL,
        PhanHoiVNPAY NVARCHAR(MAX) NULL,
        CONSTRAINT FK_GiaoDichVNPAY_ThanhToan FOREIGN KEY (IDThanhToan) REFERENCES ThanhToan(IDThanhToan)
    );
    PRINT N'[OK] Đã tạo bảng GiaoDichVNPAY.';
END
GO

-- 2.15 Bảng HocVienLopHoc (Quan hệ học viên - lớp học truyền thống)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HocVienLopHoc')
BEGIN
    CREATE TABLE HocVienLopHoc (
        IDHocVien INT NOT NULL,
        IDLopHoc NVARCHAR(50) NOT NULL,
        NgayVaoLop DATETIME DEFAULT GETDATE(),
        PRIMARY KEY (IDHocVien, IDLopHoc),
        CONSTRAINT FK_HocVienLopHoc_HocVien FOREIGN KEY (IDHocVien) REFERENCES HocVien(IDHocVien),
        CONSTRAINT FK_HocVienLopHoc_LopHoc FOREIGN KEY (IDLopHoc) REFERENCES LopHoc(IDLopHoc)
    );
    PRINT N'[OK] Đã tạo bảng HocVienLopHoc.';
END
GO

-- 2.16 Bảng DiemKhoaHoc (Điểm đánh giá khóa học trực tuyến theo thang điểm 0 - 10)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DiemKhoaHoc')
BEGIN
    CREATE TABLE DiemKhoaHoc (
        IDDiem INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        IDDangKy INT NOT NULL,
        Diem DECIMAL(4,2) NULL,
        NhanXet NVARCHAR(500) NULL,
        NgayCapNhat DATETIME NULL,
        CONSTRAINT FK_DiemKhoaHoc_DangKy FOREIGN KEY (IDDangKy) REFERENCES DangKyKhoaHoc(IDDangKy) ON DELETE CASCADE,
        CONSTRAINT UQ_DiemKhoaHoc_DangKy UNIQUE (IDDangKy),
        CONSTRAINT CK_DiemKhoaHoc_Diem CHECK (Diem IS NULL OR (Diem >= 0 AND Diem <= 10))
    );
    PRINT N'[OK] Đã tạo bảng DiemKhoaHoc.';
END
GO

-- Bảng điểm legacy (DiemIELTS, DiemTOEIC) lưu trữ nếu có
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DiemIELTS')
BEGIN
    CREATE TABLE DiemIELTS (
        IDHocVien INT NOT NULL,
        IDLopHoc NVARCHAR(50) NOT NULL,
        DiemNghe DECIMAL(3, 1) NULL,
        DiemNoi DECIMAL(3, 1) NULL,
        DiemDoc DECIMAL(3, 1) NULL,
        DiemViet DECIMAL(3, 1) NULL,
        TongDiem DECIMAL(4, 2) NULL,
        PRIMARY KEY (IDHocVien, IDLopHoc),
        CONSTRAINT FK_DiemIELTS_HocVienLopHoc FOREIGN KEY (IDHocVien, IDLopHoc) REFERENCES HocVienLopHoc(IDHocVien, IDLopHoc)
    );
    PRINT N'[OK] Đã tạo bảng DiemIELTS.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DiemTOEIC')
BEGIN
    CREATE TABLE DiemTOEIC (
        IDHocVien INT NOT NULL,
        IDLopHoc NVARCHAR(50) NOT NULL,
        Part1 INT NULL,
        Part2 INT NULL,
        Part3 INT NULL,
        Part4 INT NULL,
        DiemNghe INT NULL,
        Part5 INT NULL,
        Part6 INT NULL,
        Part7 INT NULL,
        DiemDoc INT NULL,
        DiemNoi INT NULL,
        DiemViet INT NULL,
        TongDiem INT NULL,
        PRIMARY KEY (IDHocVien, IDLopHoc),
        CONSTRAINT FK_DiemTOEIC_HocVienLopHoc FOREIGN KEY (IDHocVien, IDLopHoc) REFERENCES HocVienLopHoc(IDHocVien, IDLopHoc)
    );
    PRINT N'[OK] Đã tạo bảng DiemTOEIC.';
END
GO

-- 2.17 Bảng ThongBao
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ThongBao')
BEGIN
    CREATE TABLE ThongBao (
        IDThongBao INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        IDNguoiGui VARCHAR(50) NULL,
        TieuDe NVARCHAR(255) NULL,
        NoiDung NVARCHAR(MAX) NULL,
        NgayGui DATETIME DEFAULT GETDATE(),
        DoiTuongNhan NVARCHAR(50) NULL,
        CONSTRAINT FK_ThongBao_TaiKhoan FOREIGN KEY (IDNguoiGui) REFERENCES TaiKhoan(TenDangNhap)
    );
    PRINT N'[OK] Đã tạo bảng ThongBao.';
END
GO

-- 2.18 Bảng ChatBotNoiDung
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatBotNoiDung')
BEGIN
    CREATE TABLE ChatBotNoiDung (
        MaChat INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        CauHoiMau NVARCHAR(MAX) NULL,
        TuKhoa NVARCHAR(MAX) NULL,
        CauTraLoi NVARCHAR(MAX) NULL
    );
    PRINT N'[OK] Đã tạo bảng ChatBotNoiDung.';
END
GO

-- ==============================================================================
-- 3. DỮ LIỆU KHỞI TẠO MẶC ĐỊNH (SEED DATA)
-- ==============================================================================

-- 3.1 Khởi tạo các vai trò (Roles)
IF NOT EXISTS (SELECT * FROM LoaiTaiKhoan WHERE LoaiTK = 'admin')
    INSERT INTO LoaiTaiKhoan (LoaiTK) VALUES ('admin');
IF NOT EXISTS (SELECT * FROM LoaiTaiKhoan WHERE LoaiTK = 'giangvien')
    INSERT INTO LoaiTaiKhoan (LoaiTK) VALUES ('giangvien');
IF NOT EXISTS (SELECT * FROM LoaiTaiKhoan WHERE LoaiTK = 'hocvien')
    INSERT INTO LoaiTaiKhoan (LoaiTK) VALUES ('hocvien');
GO

-- 3.2 Khởi tạo tài khoản Quản trị viên (Admin)
-- Mật khẩu mặc định: admin@123 (đã được băm sẵn bằng chuẩn PBKDF2)
IF NOT EXISTS (SELECT * FROM TaiKhoan WHERE TenDangNhap = 'admin')
BEGIN
    INSERT INTO TaiKhoan (TenDangNhap, LoaiTK, MatKhau, Email, SDT, TrangThai)
    VALUES ('admin', 'admin', 'PBKDF2$10000$dc60c39c83ac600214d3d0930cce8f2c$17412fa0b548a042b62e444258044d3f39ee9d9a01d155a25b9b128e2c3a6a2e', 'admin@tikicourse.vn', '0988888888', N'Hoạt động');
    PRINT N'[OK] Đã tạo tài khoản quản trị admin (Mật khẩu: admin@123).';
END
GO

-- 3.3 Khởi tạo tài khoản Giảng viên tiêu biểu (Mật khẩu: 123456)
IF NOT EXISTS (SELECT * FROM TaiKhoan WHERE TenDangNhap = 'giangvien1')
BEGIN
    INSERT INTO TaiKhoan (TenDangNhap, LoaiTK, MatKhau, Email, SDT, TrangThai)
    VALUES ('giangvien1', 'giangvien', 'PBKDF2$10000$82265eeee22aa464e5d5352146dd8ead$667340524e6a8ff464e6f47bf5ad16a04bf8df3021cab49ecaa0baafc3e3fd22', 'giangvien1@tikicourse.vn', '0912345678', N'Hoạt động');

    INSERT INTO GiangVien (IDTenDangNhap, TenGV, NgaySinh, GioiTinh, DiaChi, ChuyenMon, BangCap)
    VALUES ('giangvien1', N'ThS. Nguyễn Văn Toàn', '1988-06-15', N'Nam', N'Hà Nội', N'ASP.NET MVC, C#, SQL Server, Microservices', N'Thạc sĩ Khoa học Máy tính - Đại học Bách Khoa');
    PRINT N'[OK] Đã tạo tài khoản giảng viên giangvien1 (Mật khẩu: 123456).';
END
GO

-- 3.4 Khởi tạo tài khoản Học viên tiêu biểu (Mật khẩu: 123456)
IF NOT EXISTS (SELECT * FROM TaiKhoan WHERE TenDangNhap = 'hocvien1')
BEGIN
    INSERT INTO TaiKhoan (TenDangNhap, LoaiTK, MatKhau, Email, SDT, TrangThai)
    VALUES ('hocvien1', 'hocvien', 'PBKDF2$10000$82265eeee22aa464e5d5352146dd8ead$667340524e6a8ff464e6f47bf5ad16a04bf8df3021cab49ecaa0baafc3e3fd22', 'hocvien1@gmail.com', '0905123456', N'Hoạt động');

    INSERT INTO HocVien (IDTenDangNhap, TenHV, NgaySinh, GioiTinh, DiaChi)
    VALUES ('hocvien1', N'Trần Thị Mai', '2002-11-20', N'Nữ', N'Đà Nẵng');
    PRINT N'[OK] Đã tạo tài khoản học viên hocvien1 (Mật khẩu: 123456).';
END
GO

-- 3.5 Khởi tạo danh mục khóa học
IF NOT EXISTS (SELECT * FROM DanhMucKhoaHoc WHERE TenDanhMuc = N'Lập trình Web')
BEGIN
    INSERT INTO DanhMucKhoaHoc (TenDanhMuc, MoTa, TrangThai, ThuTu) VALUES
    (N'Lập trình Web', N'Các khóa học HTML5, CSS3, JavaScript, ASP.NET MVC 5, Web API và Frontend hiện đại.', N'Hoạt động', 1),
    (N'Lập trình C# / .NET', N'Ngôn ngữ lập trình C# từ cơ bản đến nâng cao, OOP, LINQ, Entity Framework 6.', N'Hoạt động', 2),
    (N'Cơ sở dữ liệu', N'SQL Server, T-SQL, thiết kế bảng dữ liệu chuẩn hóa, tối ưu hóa truy vấn và chỉ mục Index.', N'Hoạt động', 3),
    (N'Tiếng Anh & Ngoại ngữ', N'Khóa học tiếng Anh giao tiếp công sở, luyện thi chứng chỉ TOEIC, IELTS quốc tế.', N'Hoạt động', 4),
    (N'Kỹ năng Công nghệ', N'Git, GitHub, CI/CD, phân quyền bảo mật, thanh toán trực tuyến qua cổng VNPay.', N'Hoạt động', 5);
    PRINT N'[OK] Đã tạo danh mục khóa học mặc định.';
END
GO

-- 3.6 Khởi tạo các khóa học mẫu
DECLARE @idGV INT;
SELECT TOP 1 @idGV = IDGiangVien FROM GiangVien;

IF NOT EXISTS (SELECT * FROM KhoaHoc WHERE IDKhoaHoc = 'MVC2026')
BEGIN
    INSERT INTO KhoaHoc (IDKhoaHoc, TenKhoaHoc, IDDanhMuc, IDGiangVien, DanhMuc, MoTa, NoiDung, HocPhi, HinhAnhKH, TrangThai, NgayTao)
    VALUES (
        'MVC2026', 
        N'Lập trình Web với ASP.NET MVC 5 & Entity Framework 6', 
        1, 
        @idGV,
        N'Lập trình Web', 
        N'Khóa học từ cơ bản đến nâng cao về kiến trúc ASP.NET MVC 5, Razor Engine, Entity Framework 6 Database First và tích hợp cổng thanh toán VNPay.', 
        N'<h3>Mục tiêu khóa học:</h3><ul><li>Nắm vững mô hình MVC (Model - View - Controller).</li><li>Làm chủ Entity Framework 6 Database First và SQL Server.</li><li>Xây dựng ứng dụng web học trực tuyến chuyên nghiệp.</li><li>Tích hợp thanh toán trực tuyến qua cổng VNPay Sandbox an toàn.</li></ul>', 
        499000, 
        N'mvc_banner.png', 
        N'Hiển thị', 
        GETDATE()
    );
    PRINT N'[OK] Đã tạo khóa học MVC2026.';
END

IF NOT EXISTS (SELECT * FROM KhoaHoc WHERE IDKhoaHoc = 'CSHARP01')
BEGIN
    INSERT INTO KhoaHoc (IDKhoaHoc, TenKhoaHoc, IDDanhMuc, IDGiangVien, DanhMuc, MoTa, NoiDung, HocPhi, HinhAnhKH, TrangThai, NgayTao)
    VALUES (
        'CSHARP01', 
        N'Lập trình C# Cơ Bản đến Nâng Cao cho người mới bắt đầu', 
        2, 
        @idGV,
        N'Lập trình C# / .NET', 
        N'Nền tảng lập trình C#, tư duy lập trình hướng đối tượng OOP, Generic, LINQ và xử lý ngoại lệ trong C#.', 
        N'<h3>Nội dung khóa học:</h3><p>Học lập trình C# bài bản, cấu trúc điều khiển, mảng, OOP, class, interface, LINQ to Objects.</p>', 
        0, -- Khóa học miễn phí
        N'csharp_banner.png', 
        N'Hiển thị', 
        GETDATE()
    );
    PRINT N'[OK] Đã tạo khóa học CSHARP01.';
END
GO

-- 3.7 Khởi tạo Chương học và Bài học mẫu cho khóa MVC2026
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
    (@C2, N'Bài 1: Giới thiệu Entity Framework 6 & Hướng tiếp cận Database First', N'Kết nối CSDL SQL Server và sinh file EDMX tự động', N'<p>Entity Framework Database First giúp tự động mapping bảng trong CSDL thành các C# class.</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'/Content/docs/bai4_ef6_database_first.pdf', 1, 30, 0),
    (@C2, N'Bài 2: Thao tác CRUD hoàn chỉnh với DbContext và LINQ', N'Thêm, sửa, xóa, tìm kiếm dữ liệu an toàn với Entity Framework', N'<p>Sử dụng db.KhoaHocs.Add(), db.SaveChanges(), Find() và FirstOrDefault().</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'/Content/docs/bai5_crud_linq.pdf', 2, 35, 0);

    PRINT N'[OK] Đã tạo chương học và bài học mẫu cho khóa MVC2026.';
END
GO

-- 3.8 Đồng bộ dữ liệu hiện có
UPDATE KhoaHoc SET TrangThai = N'Hiển thị' WHERE TrangThai IS NULL OR TrangThai NOT LIKE N'%Ẩn%';
UPDATE TaiKhoan SET TrangThai = N'Hoạt động' WHERE TrangThai IS NULL OR TrangThai = '';
UPDATE DangKyKhoaHoc SET TrangThai = N'Đã kích hoạt' WHERE TrangThai IS NULL OR TrangThai NOT LIKE N'%hoàn thành%' AND TrangThai NOT LIKE N'%Hủy%';
GO

-- 3.9 Khởi tạo điểm mẫu cho học viên trực tuyến
IF EXISTS (SELECT * FROM DangKyKhoaHoc WHERE IDKhoaHoc = 'MVC2026') 
   AND NOT EXISTS (SELECT * FROM DiemKhoaHoc WHERE IDDangKy IN (SELECT IDDangKy FROM DangKyKhoaHoc WHERE IDKhoaHoc = 'MVC2026'))
BEGIN
    DECLARE @idDangKyMVC INT = (SELECT TOP 1 IDDangKy FROM DangKyKhoaHoc WHERE IDKhoaHoc = 'MVC2026');
    IF @idDangKyMVC IS NOT NULL
    BEGIN
        INSERT INTO DiemKhoaHoc (IDDangKy, Diem, NhanXet, NgayCapNhat)
        VALUES (@idDangKyMVC, 8.5, N'Hoàn thành xuất sắc toàn bộ bài giảng và thực hành dự án ASP.NET MVC 5 & EF 6.', GETDATE());
        PRINT N'[OK] Đã tạo điểm mẫu cho khóa MVC2026.';
    END
END
GO

PRINT N'==============================================================================';
PRINT N'[THÀNH CÔNG] Cơ sở dữ liệu KhoaHocTrucTuyenDB đã được thiết lập hoàn chỉnh 100%!';
PRINT N'Bạn có thể khởi động ngay dự án DuAnEnglish mà không cần thực hiện thêm bước cấu hình SQL nào.';
PRINT N'==============================================================================';
GO
