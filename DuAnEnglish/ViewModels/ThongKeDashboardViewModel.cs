using System.Collections.Generic;
using DuAnEnglish.Models;

namespace DuAnEnglish.ViewModels
{
    public class ThongKeDashboardViewModel
    {
        public int TongSoHocVien { get; set; }
        public int TongSoGiangVien { get; set; }
        public int TongSoKhoaHoc { get; set; }
        public int SoKhoaHocHoatDong { get; set; }
        public int TongSoLuotDangKy { get; set; }
        public decimal TongDoanhThu { get; set; }
        public List<KhoaHocThongKeItem> TopKhoaHoc { get; set; }
        public List<ThanhToan> GiaoDichMoiNhat { get; set; }
    }

    public class KhoaHocThongKeItem
    {
        public string IDKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string TenDanhMuc { get; set; }
        public string TenGiangVien { get; set; }
        public decimal HocPhi { get; set; }
        public int SoHocVien { get; set; }
        public decimal DoanhThu { get; set; }
    }
}
