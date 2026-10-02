using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.Security;
using DuAnEnglish.ViewModels;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("admin")]
    public class HomeAdminController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: HomeAdmin
        public ActionResult Index()
        {
            int tongHocVien = db.HocViens.Count();
            int tongGiangVien = db.GiangViens.Count();
            int tongKhoaHoc = db.KhoaHocs.Count();
            int khoaHocHoatDong = db.KhoaHocs.Count(k => k.TrangThai == "Hiển thị" || k.TrangThai == null);
            int tongDangKy = db.DangKyKhoaHocs.Count();
            decimal tongDoanhThu = db.ThanhToans
                                     .Where(t => t.TrangThai == "Đã thanh toán")
                                     .Sum(t => (decimal?)t.SoTien) ?? 0;

            // Top các khóa học có nhiều học viên nhất
            var topKhoaHoc = db.KhoaHocs
                               .Include(k => k.DanhMucKhoaHoc)
                               .Include(k => k.GiangVien)
                               .ToList()
                               .Select(k => new KhoaHocThongKeItem
                               {
                                   IDKhoaHoc = k.IDKhoaHoc,
                                   TenKhoaHoc = k.TenKhoaHoc,
                                   TenDanhMuc = k.DanhMucKhoaHoc != null ? k.DanhMucKhoaHoc.TenDanhMuc : k.DanhMuc,
                                   TenGiangVien = k.GiangVien != null ? k.GiangVien.TenGV : "Chưa phân công",
                                   HocPhi = k.HocPhi ?? 0,
                                   SoHocVien = db.DangKyKhoaHocs.Count(d => d.IDKhoaHoc == k.IDKhoaHoc),
                                   DoanhThu = db.ThanhToans.Where(t => t.IDKhoaHoc == k.IDKhoaHoc && t.TrangThai == "Đã thanh toán").Sum(t => (decimal?)t.SoTien) ?? 0
                               })
                               .OrderByDescending(k => k.SoHocVien)
                               .Take(5)
                               .ToList();

            // Các giao dịch thanh toán mới nhất
            var giaoDichMoi = db.ThanhToans
                                .Include(t => t.KhoaHoc)
                                .OrderByDescending(t => t.IDThanhToan)
                                .Take(5)
                                .ToList();

            var viewModel = new ThongKeDashboardViewModel
            {
                TongSoHocVien = tongHocVien,
                TongSoGiangVien = tongGiangVien,
                TongSoKhoaHoc = tongKhoaHoc,
                SoKhoaHocHoatDong = khoaHocHoatDong,
                TongSoLuotDangKy = tongDangKy,
                TongDoanhThu = tongDoanhThu,
                TopKhoaHoc = topKhoaHoc,
                GiaoDichMoiNhat = giaoDichMoi
            };

            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}