  -- Tạo Database ---
CREATE DATABASE trungtamtienganh;

-- 1️. Bảng loai_tai_khoan (Phân quyền tài khoản) chốt
CREATE TABLE LoaiTaiKhoan(
    LoaiTK NVARCHAR(20) PRIMARY KEY not null,
);


-- 2️ Bảng tai_khoan (Quản lý tài khoản) -- chốt
CREATE TABLE TaiKhoan (
    TenDangNhap VARCHAR(50) PRIMARY KEY not null,
	LoaiTK NVARCHAR(20),
    MatKhau VARCHAR(20),
    Email VARCHAR(100),
    SDT VARCHAR(15),
    TrangThai NVARCHAR(50),
    FOREIGN KEY (LoaiTK) REFERENCES LoaiTaiKhoan(LoaiTK)
);


-- 3️ Bảng hoc_vien (Thông tin học viên) -- chốt 
CREATE TABLE HocVien (
    IDHocVien INT IDENTITY(2500,1) PRIMARY KEY not null,
    IDTenDangNhap VARCHAR(50),
	TenHV NVARCHAR(100),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
	DiaChi NVARCHAR(255),
    FOREIGN KEY (IDTenDangNhap) REFERENCES TaiKhoan(TenDangNhap)
);


-- 4️ Bảng giang_vien (Thông tin giảng viên) chốt ---
CREATE TABLE GiangVien (
    IDGiangVien INT IDENTITY(100,1) PRIMARY KEY not null,
    IDTenDangNhap VARCHAR(50),
	TenGV NVARCHAR(100),
	NgaySinh DATE,
	GioiTinh NVARCHAR(10),
	DiaChi NVARCHAR(255),
	ChuyenMon NVARCHAR(MAX),
	BangCap NVARCHAR(MAX),
    FOREIGN KEY (IDTenDangNhap) REFERENCES TaiKhoan(TenDangNhap)
);


-- 5️ Bảng khoa_hoc (Thông tin khóa học) chốt ---
CREATE TABLE KhoaHoc (
    IDKhoaHoc NVARCHAR(50) PRIMARY KEY not null ,
    TenKhoaHoc NVARCHAR(255),
	DanhMuc NVARCHAR(50), --TOEIC/ IELTS
    MoTa NVARCHAR(MAX),
    HocPhi DECIMAL(18),
	HinhAnhKH NVARCHAR(50),
);


-- 6️ Bảng phong_hoc (Quản lý phòng học) chốt ---
CREATE TABLE PhongHoc (
    IDPhongHoc INT PRIMARY KEY not null,
    TenPhong NVARCHAR(50),
    SucChua INT
);



-- 7️ Bảng lop_hoc (Thông tin lớp học) chốt---
CREATE TABLE LopHoc (
    IDLopHoc NVARCHAR(50) PRIMARY KEY not null ,
	IDPhongHoc INT,
    IDKhoaHoc NVARCHAR(50),
    IDGiangVien INT,
	TenLop VARCHAR(100),
	Slot INT,
	ThuTrongTuan NVARCHAR(50),
	GioHocBD TIME,
	GioHocKT TIME,
    FOREIGN KEY (IDKhoaHoc) REFERENCES KhoaHoc(IDKhoaHoc),
    FOREIGN KEY (IDGiangVien) REFERENCES GiangVien(IDGiangVien),
	FOREIGN KEY (IDPhongHoc) REFERENCES PhongHoc(IDPhongHoc)
);


-- 8 Bảng thanh_toan (Lịch sử thanh toán)
CREATE TABLE ThanhToan (
    IDThanhToan INT IDENTITY(202500,1) PRIMARY KEY not null,
	IDLopHoc NVARCHAR(50),
    TenDangNhap VARCHAR(50),
    IDKhoaHoc NVARCHAR(50),
    SoTien DECIMAL(18),
    PhuongThucTT NVARCHAR(50),
    NgayThanhToan DATETIME,
    TrangThai NVARCHAR(50),
	NgayXacNhan DATETIME,
	FOREIGN KEY (IDLopHoc) REFERENCES Lophoc(IDLopHoc),
    FOREIGN KEY (TenDangNhap) REFERENCES TaiKhoan(TenDangNhap),
    FOREIGN KEY (IDKhoaHoc) REFERENCES KhoaHoc(IDKhoaHoc)
);


-- 9 Bảng giao_dich_vnpay (Chi tiết giao dịch VNPay)
CREATE TABLE GiaoDichVNPAY (
    IDGiaoDich INT IDENTITY(102025,1) PRIMARY KEY not null ,
    IDThanhToan INT,
    MaGiaoDich VARCHAR(100),
    NgayGiaoDich DATETIME,
    SoTien DECIMAL(18),
    TrangThai NVARCHAR(50) ,
    NoiDung NVARCHAR(MAX),
    PhanHoiVNPAY NVARCHAR(MAX),
    FOREIGN KEY (IDThanhToan) REFERENCES ThanhToan(IDThanhToan)
);

-- 1️0 Bảng hoc_vien_lop_hoc (Danh sách học viên trong lớp)
CREATE TABLE HocVienLopHoc (
    IDHocVien INT,
    IDLopHoc NVARCHAR(50),
	PRIMARY KEY (IDHocVien, IDLopHoc),
    FOREIGN KEY (IDHocVien) REFERENCES HocVien(IDHocVien),
    FOREIGN KEY (IDLopHoc) REFERENCES LopHoc(IDLopHoc)
);

-- 1️1 Bảng ielts---
---- ielts---
CREATE TABLE DiemIELTS (
    IDHocVien INT,
    IDLopHoc NVARCHAR(50),
    DiemNghe DECIMAL(3,1),
    DiemNoi DECIMAL(3,1),
    DiemDoc DECIMAL(3,1),
    DiemViet DECIMAL(3,1),
    TongDiem DECIMAL(4,2),
    PRIMARY KEY (IDHocVien, IDLopHoc),
    FOREIGN KEY (IDHocVien, IDLopHoc) REFERENCES HocVienLopHoc(IDHocVien, IDLopHoc)
);

-- 1️2 Bảng toeic---
---toeic---
CREATE TABLE DiemTOEIC (
    IDHocVien INT,
    IDLopHoc NVARCHAR(50),
    
    -- Nghe
    Part1 INT,
    Part2 INT,
    Part3 INT,
    Part4 INT,
    DiemNghe INT,  -- tổng
    
    -- Đọc
    Part5 INT,
    Part6 INT,
    Part7 INT,
    DiemDoc INT,   -- tổng

    -- Nói & Viết
    DiemNoi INT,
    DiemViet INT,

    TongDiem INT,

    PRIMARY KEY (IDHocVien, IDLopHoc),
    FOREIGN KEY (IDHocVien, IDLopHoc) REFERENCES HocVienLopHoc(IDHocVien, IDLopHoc)
);


-- 1️3 Bảng thong_bao (Gửi thông báo)
CREATE TABLE ThongBao (
    IDThongBao INT IDENTITY(100,1) PRIMARY KEY not null,
    IDNguoiGui VARCHAR(50),
    TieuDe NVARCHAR(255),
    NoiDung NVARCHAR(MAX),
    NgayGui DATETIME,
    FOREIGN KEY (IDNguoiGui) REFERENCES TaiKhoan(TenDangNhap),
);



---14 Bang ChatBotNoiDung ---
CREATE TABLE ChatBotNoiDung(
    MaChat INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    CauHoiMau NVARCHAR(MAX),   -- Câu hỏi mẫu để admin dễ hiểu, dễ quản lý
    TuKhoa NVARCHAR(MAX),      -- Các từ khóa cách nhau bằng dấu phẩy để chatbot dò
    CauTraLoi NVARCHAR(MAX)    -- Câu trả lời mà chatbot sẽ phản hồi
);

