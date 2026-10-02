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

        // GET: QLBaoCaoThongKe/BaoCaoThongKe
        public ActionResult BaoCaoThongKe(int? thang, int? quy, int? nam)
        {
            int currentYear = nam ?? DateTime.Now.Year;

            var query = db.ThanhToans
                          .Include(t => t.KhoaHoc)
                          .Where(t => t.TrangThai == "Đã thanh toán");

            if (thang.HasValue)
            {
                query = query.Where(t => t.NgayThanhToan.HasValue &&
                                         t.NgayThanhToan.Value.Month == thang.Value &&
                                         t.NgayThanhToan.Value.Year == currentYear);
            }
            else if (quy.HasValue)
            {
                int startMonth = (quy.Value - 1) * 3 + 1;
                int endMonth = startMonth + 2;
                query = query.Where(t => t.NgayThanhToan.HasValue &&
                                         t.NgayThanhToan.Value.Month >= startMonth &&
                                         t.NgayThanhToan.Value.Month <= endMonth &&
                                         t.NgayThanhToan.Value.Year == currentYear);
            }
            else
            {
                query = query.Where(t => t.NgayThanhToan.HasValue && t.NgayThanhToan.Value.Year == currentYear);
            }

            var danhSach = query.OrderByDescending(t => t.NgayThanhToan).ToList();
            decimal tongTien = danhSach.Sum(t => t.SoTien ?? 0);

            ViewBag.SelectedThang = thang;
            ViewBag.SelectedQuy = quy;
            ViewBag.SelectedNam = currentYear;
            ViewBag.TongDoanhThuKy = tongTien;
            ViewBag.TongSoGiaoDich = danhSach.Count;

            return View(danhSach);
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