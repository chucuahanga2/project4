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
    public class QLBaoCaoThongKeController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QLBaoCaoThongKe/Index
        public ActionResult Index(int? thang, int? quy, int? nam)
        {
            return RedirectToAction("BaoCaoThongKe", new { thang, quy, nam });
        }

        // GET: QLBaoCaoThongKe/BaoCaoThongKe
        public ActionResult BaoCaoThongKe(int? thang, int? quy, int? nam)
        {
            int currentYear = nam ?? DateTime.Now.Year;

            // Load completed payments and filter in memory to avoid EF6 nullable date coalescing limitations
            var rawList = db.ThanhToans
                            .Include(t => t.KhoaHoc)
                            .Where(t => t.TrangThai == "Đã thanh toán" && (t.NgayXacNhan.HasValue || t.NgayThanhToan.HasValue))
                            .ToList();

            var filtered = rawList.Where(t =>
            {
                var dt = t.NgayXacNhan ?? t.NgayThanhToan;
                return dt.HasValue && dt.Value.Year == currentYear;
            });

            if (thang.HasValue)
            {
                filtered = filtered.Where(t => (t.NgayXacNhan ?? t.NgayThanhToan).Value.Month == thang.Value);
            }
            else if (quy.HasValue)
            {
                int startMonth = (quy.Value - 1) * 3 + 1;
                int endMonth = startMonth + 2;
                filtered = filtered.Where(t =>
                {
                    int m = (t.NgayXacNhan ?? t.NgayThanhToan).Value.Month;
                    return m >= startMonth && m <= endMonth;
                });
            }

            var danhSach = filtered.OrderByDescending(t => t.NgayXacNhan ?? t.NgayThanhToan).ToList();
            decimal tongTien = danhSach.Sum(t => t.SoTien ?? 0);

            ViewBag.SelectedThang = thang;
            ViewBag.SelectedQuy = quy;
            ViewBag.SelectedNam = currentYear;
            ViewBag.TongDoanhThuKy = tongTien;
            ViewBag.TongSoGiaoDich = danhSach.Count;

            return View("BaoCaoThongKe", danhSach);
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