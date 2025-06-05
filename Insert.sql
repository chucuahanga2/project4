-- insert Loaitaikhoan ----
INSERT INTO LoaiTaiKhoan (LoaiTK) VALUES ('admin');
INSERT INTO LoaiTaiKhoan (LoaiTK) VALUES ('giangvien');
INSERT INTO LoaiTaiKhoan (LoaiTK) VALUES ('hocvien');

--- insert  taikhoan --- chốt
INSERT INTO TaiKhoan (TenDangNhap, LoaiTK, MatKhau, Email, SDT, TrangThai) 
VALUES ('admin', 'admin', '123', 'admin01@gmail.com', '0935984444', N'Hoạt động')
INSERT INTO TaiKhoan (TenDangNhap, LoaiTK, MatKhau, Email, SDT, TrangThai) 
VALUES ('phuoc', 'giangvien', '123', 'giangvien@gmail.com', '0935555555', N'Hoạt động')
INSERT INTO TaiKhoan (TenDangNhap, LoaiTK, MatKhau, Email, SDT, TrangThai) 
VALUES ('phuc', 'hocvien', '123', 'phuc@gmail.com', '0355444444', N'Hoạt động')
INSERT INTO TaiKhoan (TenDangNhap, LoaiTK, MatKhau, Email, SDT, TrangThai) 
VALUES ('thanh', 'giangvien', '123', 'thanh@gmail.com', '0935989889', N'Khóa')


-- insert
INSERT INTO HocVien (IDTenDangNhap, TenHV, NgaySinh, GioiTinh, DiaChi) 
VALUES ('phuc', N'Nguyễn Hoàng Phúc', '2002/01/01', N'Nam',N'Đại Lộc Quảng Nam')
INSERT INTO HocVien (IDTenDangNhap, TenHV, NgaySinh, GioiTinh, DiaChi) 
VALUES ('thanh', N'Nguyễn Thúy Thanh', '2002/01/01', N'Nữ',N'Quế Sơn Quảng Nam')

-- Insert GiangVien
INSERT INTO GiangVien (IDTenDangNhap, TenGV, NgaySinh, GioiTinh, DiaChi) 
VALUES ('phuoc', N'Huỳnh Phước', '2003/01/01', N'Nam', N'Điện Bàn Quảng Nam');

INSERT INTO GiangVien (IDTenDangNhap, TenGV, NgaySinh, GioiTinh, DiaChi) 
VALUES ('thanh', N'Nguyễn Thúy Thanh', '2003/01/01', N'Nữ', N'Điện Bàn Quảng Nam');

--- insert  KhoaHoc --- chốt
INSERT INTO KhoaHoc(IDKhoaHoc, TenKhoaHoc, DanhMuc, MoTa, HocPhi, HinhAnhKH) 
VALUES ('TIA2025', N'TOEIC Cơ Bản', N'TOEIC', N'Cung cấp kiến thức cơ bản về TOEIC, 
tập trung vào kỹ năng Nghe và Đọc. Giúp người học làm quen với định dạng đề thi 
và các dạng câu hỏi thường gặp.', '2000000', N'TI_CoBan.jpg')

INSERT INTO KhoaHoc(IDKhoaHoc, TenKhoaHoc, DanhMuc, MoTa, HocPhi, HinhAnhKH) 
VALUES ('TIB2025', N'TOEIC Trung Cấp', N'TOEIC', N'Hướng đến người học đã có nền tảng
và muốn nâng cao kỹ năng làm bài TOEIC. Bổ sung phương pháp làm bài hiệu quả, tăng cường
vốn từ vựng và chiến lược xử lý bài thi.', '3000000', N'TI_TrungCap.jpg')

INSERT INTO KhoaHoc(IDKhoaHoc, TenKhoaHoc, DanhMuc, MoTa, HocPhi, HinhAnhKH) 
VALUES ('TILD2025', N'TOEIC Luyện Đề', N'TOEIC', N'Được thiết kế để ôn luyện thông 
qua đề thi thực tế, giúp người học làm quen với cấu trúc đề thi TOEIC và rèn luyện 
kỹ năng giải đề trong thời gian quy định.', '2000000', N'TI_LuyenDe.jpg')

INSERT INTO KhoaHoc(IDKhoaHoc, TenKhoaHoc, DanhMuc, MoTa, HocPhi, HinhAnhKH) 
VALUES ('TIMT2025', N'TOEIC Master', N'TOEIC', N'Khóa học chuyên sâu dành cho người 
muốn đạt điểm TOEIC cao, tập trung vào mẹo và chiến lược giúp tối ưu hóa điểm số 
trong kỳ thi chính thức.', '4000000', N'TI_Master.jpg')

INSERT INTO KhoaHoc(IDKhoaHoc, TenKhoaHoc, DanhMuc, MoTa, HocPhi, HinhAnhKH) 
VALUES ('IEA2025', N'IELTS Cơ Bản', N'IELTS', N'Khóa học giúp người học xây dựng nền 
tảng vững chắc về IELTS, bao gồm các kỹ năng Nghe, Nói, Đọc, Viết. Phù hợp với người 
mới bắt đầu hoặc muốn cải thiện căn bản.', '2000000', N'NULL')

INSERT INTO KhoaHoc(IDKhoaHoc, TenKhoaHoc, DanhMuc, MoTa, HocPhi, HinhAnhKH) 
VALUES ('IEA2026', N'IELTS Cơ Bản', N'IELTS', N'Khóa học giúp người học xây dựng nền 
tảng vững chắc về IELTS, bao gồm các kỹ năng Nghe, Nói, Đọc, Viết. Phù hợp với người 
mới bắt đầu hoặc muốn cải thiện căn bản.', '2000000', N'NULL')

-- Thêm dữ liệu vào bảng PhongHoc
INSERT INTO PhongHoc (IDPhongHoc, TenPhong, SucChua) VALUES
(101, N'TOEIC 1', 20),
(102, N'TOEIC 2', 20),
(201, N'TOEIC 3', 20),
(202, N'TOEIC Master', 20),
(301, N'IELTS 1', 20),
(302, N'IELTS Master', 20),
(401, 'Toeic LD', 20),
(402, 'Ielts LD', 20);

-- Insert LopHoc
INSERT INTO LopHoc(IDLopHoc, IDPhongHoc, IDKhoaHoc, IDGiangVien, TenLop, Slot, ThuTrongTuan, GioHocBD, GioHocKT)
VALUES 
('LH007', 301, 'IEA2026', NULL, N'IEA2', 20, N'Thứ 3, Thứ 5', '18:30:00', '20:30:00');

('LH001', NULL, 'TIA2025', NULL, N'TIA1', 0, N'Thứ 2, Thứ 4', '08:00:00', '10:00:00'),

('LH006', 101, 'TIA2025', NULL, N'TIA2', 20, N'Thứ 3, Thứ 5', '08:00:00', '10:00:00'),

('LH002', NULL, 'TIB2025', NULL, N'TIB1', 20, N'Thứ 3, Thứ 5', '13:30:00', '15:30:00'),

('LH003', NULL, 'TILD2025', NULL, N'TILD1', 20, N'Thứ 2, Thứ 6', '18:00:00', '20:00:00'),

('LH004', NULL, 'TIMT2025', NULL, N'TIMT1', 20, N'Thứ 7, Chủ Nhật', '09:00:00', '11:00:00'),

('LH005', 301, 'IEA2025', NULL, N'IEA1', 20, N'Thứ 3, Thứ 5', '18:30:00', '20:30:00');


INSERT INTO HocVienLophoc (IDHocVien, IDLopHoc) 
VALUES ('2500', 'LH006')
INSERT INTO HocVienLophoc (IDHocVien, IDLopHoc) 
VALUES ('2501', 'LH006')
INSERT INTO HocVienLophoc (IDHocVien, IDLopHoc) 
VALUES ('2500', 'LH005')
INSERT INTO HocVienLophoc (IDHocVien, IDLopHoc) 
VALUES ('2501', 'LH005')



-- Học viên 2500: điểm IELTS
INSERT INTO DiemIELTS (IDHocVien, IDLopHoc, DiemNghe, DiemNoi, DiemDoc, DiemViet, TongDiem)
VALUES (2500, 'LH005', 6.5, 6.0, 6.5, 6.0, 6.25);
-- Học viên 2501: điểm IELTS
INSERT INTO DiemIELTS (IDHocVien, IDLopHoc, DiemNghe, DiemNoi, DiemDoc, DiemViet, TongDiem)
VALUES (2501, 'LH005', 5.5, 6.0, 5.5, 5.0, 5.50);
INSERT INTO DiemIELTS (IDHocVien, IDLopHoc, DiemNghe, DiemNoi, DiemDoc, DiemViet, TongDiem)
VALUES (2503, 'LH005', 5.5, 6.0, 5.5, 5.0, 5.50);

-- Học viên 2500
INSERT INTO DiemTOEIC (IDHocVien, IDLopHoc, Part1, Part2, Part3, Part4, DiemNghe, Part5, Part6, Part7, DiemDoc, DiemNoi, DiemViet, TongDiem)
VALUES (2500, 'LH006', 5, 10, 10, 10, 300, 10, 10, 10, 250, 0, 0, 550);

-- Học viên 2501
INSERT INTO DiemTOEIC (IDHocVien, IDLopHoc, Part1, Part2, Part3, Part4, DiemNghe, Part5, Part6, Part7, DiemDoc, DiemNoi, DiemViet, TongDiem)
VALUES (2501, 'LH006', 6, 8, 12, 14, 350, 11, 12, 10, 270, 0, 0, 620);


-- Thông báo gửi toàn hệ thống (IDNguoiNhan = NULL)
INSERT INTO ThongBao (IDNguoiGui,TieuDe, NoiDung, NgayGui)
VALUES ('admin', N'Thông báo chung', N'Hệ thống sẽ bảo trì vào cuối tuần này.', '2025-05-07 08:00:00');

-- Thông báo gửi toàn hệ thống (IDNguoiNhan = NULL)
INSERT INTO ThongBao (IDNguoiGui,TieuDe, NoiDung, NgayGui)
VALUES ('admin', N'Chúc mừng năm mới', N'Chúc tất cả các bạn một năm mới an khang thịnh vượng!', '2025-01-01 00:00:00');

INSERT INTO ThongBao (IDNguoiGui, TieuDe, NoiDung, NgayGui)
VALUES 
('admin', N'Thông báo lịch nghỉ Tết', N'Trung tâm sẽ nghỉ Tết từ ngày 06/02 đến 12/02.', '2025-01-05 08:30:00'),
('admin', N'Khai giảng lớp mới', N'Lớp Tiếng Anh giao tiếp khai giảng vào ngày 15/01.', '2025-01-08 09:00:00'),
('admin', N'Thông báo học bù', N'Học viên lớp A1 học bù vào thứ Bảy tuần này lúc 9h.', '2025-01-10 14:00:00'),
('admin', N'Cập nhật giáo trình', N'Giáo trình mới đã được cập nhật trên hệ thống.', '2025-01-12 10:15:00'),
('admin', N'Cuộc thi tiếng Anh', N'Cuộc thi English Challenge sẽ tổ chức ngày 20/01.', '2025-01-13 16:00:00'),
('admin', N'Thông báo đóng học phí', N'Vui lòng đóng học phí học kỳ 2 trước ngày 25/01.', '2025-01-15 11:45:00'),
('admin', N'Nhận tài liệu ôn tập', N'Tài liệu ôn tập cuối kỳ đã sẵn sàng để tải về.', '2025-01-17 13:30:00'),
('admin', N'Lịch kiểm tra giữa kỳ', N'Bài kiểm tra giữa kỳ sẽ diễn ra vào ngày 28/01.', '2025-01-18 15:00:00'),
('admin', N'Thông báo họp phụ huynh', N'Thời gian họp phụ huynh: 26/01 lúc 18h.', '2025-01-20 17:30:00'),
('admin', N'Thêm khóa học mới', N'Đã mở đăng ký lớp TOEIC nâng cao, khai giảng 05/02.', '2025-01-22 08:00:00'),
('admin', N'Khảo sát ý kiến học viên', N'Vui lòng hoàn thành khảo sát trước ngày 30/01.', '2025-01-23 10:00:00'),
('admin', N'Thông báo nhận bằng', N'Học viên khóa 12 có thể nhận bằng từ ngày 01/02.', '2025-01-24 09:45:00'),
('admin', N'Thay đổi lịch học', N'Lịch học thứ Hai chuyển sang thứ Ba tuần này.', '2025-01-25 12:15:00'),
('admin', N'Khuyến mãi đặc biệt', N'Giảm 20% học phí cho học viên đăng ký trước 10/02.', '2025-01-26 14:30:00'),
('admin', N'Chúc mừng sinh nhật', N'Trung tâm chúc mừng sinh nhật tất cả học viên trong tháng 1!', '2025-01-27 18:00:00');



-- Câu hỏi về khóa học tiếng Anh
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES 
(N'Khóa học tiếng Anh có mức học phí bao nhiêu?', N'học phí, bao nhiêu, khóa học, tiếng Anh', N'Khóa học tiếng Anh có mức học phí từ 1,000,000 VND đến 3,000,000 VND tuỳ thuộc vào khóa học cụ thể.'),

-- Câu hỏi về các lớp học
(N'Có lớp học vào cuối tuần không?', N'lớp học, cuối tuần, có không', N'Chúng tôi có các lớp học vào cuối tuần cho bạn thoải mái lựa chọn. Liên hệ chúng tôi để biết thêm chi tiết.'),

-- Câu hỏi về các giảng viên
(N'Giảng viên dạy khóa học này là ai?', N'giảng viên, dạy khóa học', N'Giảng viên dạy khóa học này là cô Nguyễn Thị Mai, người có hơn 10 năm kinh nghiệm giảng dạy tiếng Anh.')

-- Nên chọn khóa nào cho người mới
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'Tôi mới bắt đầu học tiếng Anh, nên học khóa nào?', N'mới bắt đầu, học, nên chọn, khóa nào', 
N'Bạn có thể bắt đầu với khóa TOEIC Cơ Bản hoặc IELTS Cơ Bản để xây dựng nền tảng vững chắc.');

-- Khóa nào phù hợp nếu muốn thi TOEIC
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'Tôi muốn luyện thi TOEIC thì học khóa nào?', N'TOEIC, luyện thi, nên học, khóa nào', 
N'Trung tâm có các khóa TOEIC từ cơ bản đến nâng cao như: TOEIC Cơ Bản, Trung Cấp, Luyện Đề, và Master để bạn lựa chọn theo trình độ.');

-- Lớp học TOEIC có những ngày nào
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'Lớp TOEIC học vào ngày nào?', N'lớp, TOEIC, ngày học, lịch học', 
N'Lớp TOEIC có nhiều lịch học: T2-T4 (08:00–10:00), T3-T5 (08:00–10:00 hoặc 13:30–15:30), T2-T6 (18:00–20:00), và T7-CN (09:00–11:00).');

-- Lớp IELTS học mấy giờ
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'Lớp IELTS học lúc mấy giờ?', N'IELTS, lớp học, giờ học, mấy giờ', 
N'Lớp IELTS có giờ học buổi tối: T3-T5, từ 18:30 đến 20:30.');

-- Có lớp buổi tối không
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'Trung tâm có lớp học buổi tối không?', N'buổi tối, lớp học, có không', 
N'Trung tâm có các lớp học buổi tối từ 18:00–20:30 vào các ngày trong tuần như Thứ 2, 3, 5, 6.');

-- Hỏi có lớp cuối tuần không
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'Tôi chỉ rảnh cuối tuần, có lớp nào không?', N'lớp, cuối tuần, thứ 7, chủ nhật', 
N'Trung tâm có lớp TOEIC Master học vào Thứ 7 và Chủ Nhật, từ 09:00 đến 11:00.');

INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'Học phí tại trung tâm là bao nhiêu?', N'trung tâm, học phí tại trung tâm', 
N'Học phí dao động từ 2,000,000 đến 4,000,000 VND tuỳ vào từng khóa học cụ thể như TOEIC hay IELTS.');

-- TOEIC Cơ Bản
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'Khóa TOEIC Cơ Bản học phí bao nhiêu?', N'TOEIC, cơ bản, học phí, giá', 
N'Khóa TOEIC Cơ Bản có học phí là 2,000,000 VND.');

-- TOEIC Trung Cấp
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'TOEIC Trung Cấp bao nhiêu tiền?', N'TOEIC, trung cấp, giá, học phí', 
N'Khóa TOEIC Trung Cấp có học phí là 3,000,000 VND.');

-- TOEIC Luyện Đề
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'Học TOEIC luyện đề giá thế nào?', N'TOEIC, luyện đề, giá, học phí', 
N'Khóa TOEIC Luyện Đề có học phí là 2,000,000 VND.');

-- TOEIC Master
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'TOEIC Master học phí bao nhiêu?', N'TOEIC, master, giá, học phí', 
N'Khóa TOEIC Master có học phí là 4,000,000 VND.');

-- IELTS Cơ Bản
INSERT INTO ChatBotNoiDung (CauHoiMau, TuKhoa, CauTraLoi)
VALUES (N'Học phí khóa IELTS cơ bản là bao nhiêu?', N'IELTS, cơ bản, giá, học phí', 
N'Khóa IELTS Cơ Bản có học phí là 2,000,000 VND.');
