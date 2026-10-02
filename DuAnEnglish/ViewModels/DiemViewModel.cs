using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAnEnglish.ViewModels
{
    public class DiemViewModel
    {
        public int IDHocVien { get; set; }
        public string IDLopHoc { get; set; }

        public decimal? DiemNgheIELTS { get; set; }
        public decimal? DiemNoiIELTS { get; set; }
        public decimal? DiemDocIELTS { get; set; }
        public decimal? DiemVietIELTS { get; set; }
        public decimal? TongDiemIELTS { get; set; }

        public int? DiemNgheTOEIC { get; set; }
        public int? DiemDocTOEIC { get; set; }
        public int? DiemNoiTOEIC { get; set; }
        public int? DiemVietTOEIC { get; set; }
        public int? TongDiemTOEIC { get; set; }

        // Thuộc tính chung để View dễ dùng
        public decimal? DiemNghe { get { return DiemNgheIELTS ?? (DiemNgheTOEIC.HasValue ? (decimal?)DiemNgheTOEIC.Value : null); } }
        public decimal? DiemNoi { get { return DiemNoiIELTS ?? (DiemNoiTOEIC.HasValue ? (decimal?)DiemNoiTOEIC.Value : null); } }
        public decimal? DiemDoc { get { return DiemDocIELTS ?? (DiemDocTOEIC.HasValue ? (decimal?)DiemDocTOEIC.Value : null); } }
        public decimal? DiemViet { get { return DiemVietIELTS ?? (DiemVietTOEIC.HasValue ? (decimal?)DiemVietTOEIC.Value : null); } }
        public decimal? TongDiem { get { return TongDiemIELTS ?? (TongDiemTOEIC.HasValue ? (decimal?)TongDiemTOEIC.Value : null); } }

        public string DanhMuc { get; set; }
    }
}