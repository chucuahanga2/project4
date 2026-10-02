using System.Collections.Generic;
using DuAnEnglish.Models;

namespace DuAnEnglish.ViewModels
{
    public class ChiTietKhoaHocViewModel
    {
        public KhoaHoc KhoaHoc { get; set; }
        public List<ChuongHocItem> DanhSachChuong { get; set; }
        public int TongSoBaiHoc { get; set; }
        public int TongSoBai { get { return TongSoBaiHoc; } set { TongSoBaiHoc = value; } }
        public int TongThoiLuong { get; set; }
        public int TongThoiLuongPhut { get { return TongThoiLuong; } set { TongThoiLuong = value; } }
        public bool DaDangKy { get; set; }
        public bool DaKichHoat { get; set; }
        public int PhanTramTienDo { get; set; }
        public List<KhoaHoc> KhoaHocLienQuan { get; set; }
    }
}
