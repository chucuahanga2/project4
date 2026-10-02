using System;
using System.Collections.Generic;
using DuAnEnglish.Models;

namespace DuAnEnglish.ViewModels
{
    public class KhoaHocCuaToiItem
    {
        public string IDKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string HinhAnhKH { get; set; }
        public string TenGiangVien { get; set; }
        public DateTime? NgayDangKy { get; set; }
        public int TongSoBai { get; set; }
        public int SoBaiDaHoanThanh { get; set; }
        public int PhanTramTienDo
        {
            get
            {
                if (TongSoBai == 0) return 0;
                return (int)Math.Round((double)SoBaiDaHoanThanh / TongSoBai * 100);
            }
        }
        public int? IDBaiHocTiepTheo { get; set; }
        public string TenBaiHocTiepTheo { get; set; }
        public decimal? Diem { get; set; }
        public string NhanXet { get; set; }
        public DateTime? NgayCapNhatDiem { get; set; }
    }

    public class VaoHocViewModel
    {
        public KhoaHoc KhoaHoc { get; set; }
        public BaiHoc BaiHocHienTai { get; set; }
        public List<ChuongHocItem> DanhSachChuong { get; set; }
        public int TongSoBai { get; set; }
        public int SoBaiDaHoanThanh { get; set; }
        public int PhanTramTienDo
        {
            get
            {
                if (TongSoBai == 0) return 0;
                return (int)Math.Round((double)SoBaiDaHoanThanh / TongSoBai * 100);
            }
        }
        public bool DaHoanThanhBaiHienTai { get; set; }
        public BaiHoc BaiHocTruoc { get; set; }
        public BaiHoc BaiHocSau { get; set; }
    }

    public class ChuongHocItem
    {
        public ChuongHoc Chuong { get; set; }
        public List<BaiHocItem> DanhSachBai { get; set; }
        public List<BaiHocItem> DanhSachBaiHoc { get { return DanhSachBai; } set { DanhSachBai = value; } }
    }

    public class BaiHocItem
    {
        public BaiHoc Bai { get; set; }
        public bool DaHoanThanh { get; set; }
        public bool DangHoc { get; set; }
    }
}
