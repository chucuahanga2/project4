$connString = "Server=localhost;Database=KhoaHocTrucTuyenDB;Integrated Security=True;Connection Timeout=30;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()
Write-Host "Connected to KhoaHocTrucTuyenDB..."

function Exec-SQL($sql, $desc) {
    Write-Host "Executing: $desc..."
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    try {
        $cmd.ExecuteNonQuery() | Out-Null
        Write-Host "-> Success: $desc"
    } catch {
        Write-Host "-> Error in $desc - " $_.Exception.Message
    }
}

# 1. Bảng BaiHoc
Exec-SQL @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'BaiHoc')
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
END
"@ "Create BaiHoc table"

# 2. Bảng DangKyKhoaHoc
Exec-SQL @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DangKyKhoaHoc')
BEGIN
    CREATE TABLE DangKyKhoaHoc (
        IDDangKy INT IDENTITY(1000,1) PRIMARY KEY NOT NULL,
        IDHocVien INT NOT NULL,
        IDKhoaHoc VARCHAR(50) NOT NULL,
        NgayDangKy DATETIME DEFAULT GETDATE(),
        TrangThai NVARCHAR(50) DEFAULT N'Chưa kích hoạt',
        NgayHoanThanh DATETIME NULL,
        CONSTRAINT FK_DangKy_HocVien FOREIGN KEY (IDHocVien) REFERENCES HocVien(IDHocVien),
        CONSTRAINT FK_DangKy_KhoaHoc FOREIGN KEY (IDKhoaHoc) REFERENCES KhoaHoc(IDKhoaHoc),
        CONSTRAINT UQ_HocVien_KhoaHoc UNIQUE (IDHocVien, IDKhoaHoc)
    );
END
"@ "Create DangKyKhoaHoc table"

# 3. Bảng TienDoHoc
Exec-SQL @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TienDoHoc')
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
END
"@ "Create TienDoHoc table"

# 4. Cột IDDangKy trong ThanhToan
Exec-SQL @"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('ThanhToan') AND name = 'IDDangKy')
BEGIN
    ALTER TABLE ThanhToan ADD IDDangKy INT NULL;
    ALTER TABLE ThanhToan ADD CONSTRAINT FK_ThanhToan_DangKy FOREIGN KEY (IDDangKy) REFERENCES DangKyKhoaHoc(IDDangKy);
END
"@ "Add IDDangKy to ThanhToan"

# 5. Dữ liệu mẫu bài giảng khóa MVC2026
Exec-SQL @"
IF NOT EXISTS (SELECT 1 FROM BaiHoc)
BEGIN
    DECLARE @ch1 INT, @ch2 INT;
    SELECT TOP 1 @ch1 = IDChuong FROM ChuongHoc WHERE IDKhoaHoc = 'MVC2026' ORDER BY ThuTu ASC;
    SELECT TOP 1 @ch2 = IDChuong FROM ChuongHoc WHERE IDKhoaHoc = 'MVC2026' ORDER BY ThuTu DESC;

    IF (@ch1 IS NOT NULL)
    BEGIN
        INSERT INTO BaiHoc (IDChuong, TenBaiHoc, MoTa, NoiDung, VideoUrl, TaiLieuUrl, ThuTu, ThoiLuong, ChoXemThu) VALUES
        (@ch1, N'Bài 1: Giới thiệu mô hình MVC và các thành phần cốt lõi', N'Tìm hiểu tương tác Model - View - Controller', N'<p>MVC là kiến trúc chuẩn cho ứng dụng web hiện đại.</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'https://github.com', 1, 15, 1),
        (@ch1, N'Bài 2: Cấu trúc thư mục dự án ASP.NET MVC 5', N'Ý nghĩa các thư mục Controllers, Views, Models...', N'<p>Thư mục App_Start chứa RouteConfig.</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'https://github.com', 2, 20, 1),
        (@ch1, N'Bài 3: Controller và Action: Xử lý Request và Return View', N'Cách tạo Controller, định nghĩa Action', N'<p>Action trả về ViewResult.</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'https://github.com', 3, 25, 0);
    END

    IF (@ch2 IS NOT NULL AND @ch2 <> @ch1)
    BEGIN
        INSERT INTO BaiHoc (IDChuong, TenBaiHoc, MoTa, NoiDung, VideoUrl, TaiLieuUrl, ThuTu, ThoiLuong, ChoXemThu) VALUES
        (@ch2, N'Bài 1: Giới thiệu Entity Framework 6 & Database First', N'Kết nối SQL Server và sinh model', N'<p>EF Database First giúp sinh model từ CSDL.</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'https://github.com', 1, 30, 0),
        (@ch2, N'Bài 2: Thao tác CRUD hoàn chỉnh với DbContext và LINQ', N'Thêm, sửa, xóa, tìm kiếm dữ liệu', N'<p>Sử dụng LINQ to Entities.</p>', N'https://www.youtube.com/embed/dQw4w9WgXcQ', N'https://github.com', 2, 35, 0);
    END
END
"@ "Seed sample lessons"

# 6. Đăng ký mẫu cho học viên 'phuc'
Exec-SQL @"
DECLARE @hvId INT;
SELECT @hvId = IDHocVien FROM HocVien WHERE IDTenDangNhap = 'phuc';
IF (@hvId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DangKyKhoaHoc WHERE IDHocVien = @hvId AND IDKhoaHoc = 'MVC2026'))
BEGIN
    INSERT INTO DangKyKhoaHoc (IDHocVien, IDKhoaHoc, NgayDangKy, TrangThai)
    VALUES (@hvId, 'MVC2026', GETDATE(), N'Đã kích hoạt');

    -- Đánh dấu hoàn thành bài 1
    DECLARE @bai1 INT;
    SELECT TOP 1 @bai1 = IDBaiHoc FROM BaiHoc ORDER BY IDBaiHoc ASC;
    IF (@bai1 IS NOT NULL)
    BEGIN
        INSERT INTO TienDoHoc (IDHocVien, IDBaiHoc, DaHoanThanh, NgayHoanThanh, ThoiDiemXemGanNhat)
        VALUES (@hvId, @bai1, 1, GETDATE(), GETDATE());
    END
END
"@ "Seed sample enrollment for student phuc"

$conn.Close()
Write-Host "All tables and seed data completed!"
