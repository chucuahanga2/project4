using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class HomeController : Controller
    {
        // Khởi tạo DbContext
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: Home
        public ActionResult Index()
        {
            // Lấy toàn bộ danh sách khóa học từ cơ sở dữ liệu
            var dsKhoaHoc = db.KhoaHocs.ToList();

            // Truyền danh sách khóa học vào view
            return View(dsKhoaHoc);
        }
        public ActionResult KhoaHoc()
        {
            return View();
        }
        public ActionResult ChiaSe()
        {
            return View();
        }
        public ActionResult SuKien()
        {
            return View();
        }
        public ActionResult LienHe()
        {
            return View();
        }
        public ActionResult TinTuc()
        {
            return View();
        }
        public ActionResult TuyenDung()
        {
            return View();
        }
    }
}