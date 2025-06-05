using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class QLBaoCaoThongKeController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: QLBaoCaoThongKe
        public ActionResult BaoCaoThongKe(int? thang, int? quy, int? nam)
        {
            // Lấy những thanh toán đã thanh toán
            var query = db.ThanhToans.Where(t => t.TrangThai == "Đã thanh toán");

            if (nam.HasValue)
            {
                if (thang.HasValue)
                {
                    query = query.Where(t => t.NgayThanhToan.HasValue &&
                                             t.NgayThanhToan.Value.Month == thang.Value &&
                                             t.NgayThanhToan.Value.Year == nam.Value);
                }
                else if (quy.HasValue)
                {
                    int startMonth = (quy.Value - 1) * 3 + 1;
                    int endMonth = startMonth + 2;

                    query = query.Where(t => t.NgayThanhToan.HasValue &&
                                             t.NgayThanhToan.Value.Month >= startMonth &&
                                             t.NgayThanhToan.Value.Month <= endMonth &&
                                             t.NgayThanhToan.Value.Year == nam.Value);
                }
                else
                {
                    query = query.Where(t => t.NgayThanhToan.HasValue &&
                                             t.NgayThanhToan.Value.Year == nam.Value);
                }
            }

            var danhSach = query.ToList();

            return View(danhSach);
        }
    }
}