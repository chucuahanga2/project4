using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;
namespace DuAnEnglish.Controllers
{
    public class LopHocController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // Ham hien thi danh sach lop hoc
        // GET: LopHoc
        public ActionResult DanhSachLopHoc(string id)
        {
            
            var DanhSachLopHoc = db.LopHocs
                                .Where(l => l.IDKhoaHoc == id)
                                .ToList();

            ViewBag.TenKhoaHoc = db.KhoaHocs
                                   .Where(k => k.IDKhoaHoc == id)
                                   .Select(k => k.TenKhoaHoc)
                                   .FirstOrDefault();
            // CHUYỂN TempData sang ViewBag để view hiển thị được
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            return View(DanhSachLopHoc);
        }

    }
}