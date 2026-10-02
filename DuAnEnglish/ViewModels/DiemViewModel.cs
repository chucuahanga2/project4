using System;

namespace DuAnEnglish.ViewModels
{
    public class DiemViewModel
    {
        public int IDDangKy { get; set; }
        public int IDHocVien { get; set; }
        public string TenHocVien { get; set; }
        public string TenDangNhap { get; set; }
        public string IDKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public int TienDoPhanTram { get; set; }
        public int SoBaiDaHoc { get; set; }
        public int TongSoBaiHoc { get; set; }
        public decimal? Diem { get; set; }
        public string NhanXet { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public string TrangThaiDangKy { get; set; }
    }

    public class NhapDiemViewModel
    {
        public int IDDangKy { get; set; }
        public int IDHocVien { get; set; }
        public string TenHocVien { get; set; }
        public string TenDangNhap { get; set; }
        public string IDKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public int TienDoPhanTram { get; set; }
        public int SoBaiDaHoc { get; set; }
        public int TongSoBaiHoc { get; set; }
        public decimal? Diem { get; set; }
        public string NhanXet { get; set; }
        public DateTime? NgayCapNhat { get; set; }
    }
}