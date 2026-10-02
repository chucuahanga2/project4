-- ==============================================================================
-- SCRIPT CHUẨN HÓA VÀ NÂNG CẤP CƠ SỞ DỮ LIỆU: KHOA HỌC TRỰC TUYẾN
-- Database: KhoaHocTrucTuyenDB
-- Tính chất: Idempotent (Chạy an toàn nhiều lần không gây lỗi trùng lặp)
-- ==============================================================================

USE KhoaHocTrucTuyenDB;
GO

SET NOCOUNT ON;

-- 1. Mở rộng độ dài cột MatKhau trong bảng TaiKhoan để hỗ trợ băm mật khẩu
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TaiKhoan' AND COLUMN_NAME = 'MatKhau' AND CHARACTER_MAXIMUM_LENGTH < 256)
BEGIN
    ALTER TABLE TaiKhoan ALTER COLUMN MatKhau VARCHAR(256) NOT NULL;
    PRINT N'[OK] Đã mở rộng cột TaiKhoan.MatKhau lên VARCHAR(256).';
END
GO

-- 2. Đồng bộ TrangThai của bảng KhoaHoc (chuyển các khóa không phải Ẩn sang Hiển thị)
UPDATE KhoaHoc SET TrangThai = N'Hiển thị' WHERE TrangThai IS NULL OR TrangThai NOT LIKE N'%Ẩn%';
PRINT N'[OK] Đã chuẩn hóa TrangThai bảng KhoaHoc thành N''Hiển thị''.';
GO

-- 3. Đảm bảo trạng thái tài khoản
UPDATE TaiKhoan SET TrangThai = N'Hoạt động' WHERE TrangThai IS NULL OR TrangThai = '';
PRINT N'[OK] Đã chuẩn hóa TrangThai bảng TaiKhoan.';
GO

-- 4. Đảm bảo các ràng buộc UNIQUE cốt lõi cho Đăng ký và Tiến độ
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_HocVien_KhoaHoc' AND object_id = OBJECT_ID('DangKyKhoaHoc'))
BEGIN
    ALTER TABLE DangKyKhoaHoc ADD CONSTRAINT UQ_HocVien_KhoaHoc UNIQUE (IDHocVien, IDKhoaHoc);
    PRINT N'[OK] Đã bổ sung ràng buộc UNIQUE (IDHocVien, IDKhoaHoc) cho DangKyKhoaHoc.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_HocVien_BaiHoc' AND object_id = OBJECT_ID('TienDoHoc'))
BEGIN
    ALTER TABLE TienDoHoc ADD CONSTRAINT UQ_HocVien_BaiHoc UNIQUE (IDHocVien, IDBaiHoc);
    PRINT N'[OK] Đã bổ sung ràng buộc UNIQUE (IDHocVien, IDBaiHoc) cho TienDoHoc.';
END
GO

PRINT N'==============================================================================';
PRINT N'[HOÀN TẤT] Cập nhật Database KhoaHocTrucTuyenDB thành công!';
PRINT N'==============================================================================';
GO
