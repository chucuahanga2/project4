using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class HomeController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: Home/Index
        public ActionResult Index()
        {
            var dsKhoaHocMoi = db.KhoaHocs
                                 .Include(k => k.DanhMucKhoaHoc)
                                 .Include(k => k.GiangVien)
                                 .Include(k => k.DangKyKhoaHocs)
                                 .Where(k => k.TrangThai == "Hiển thị" || k.TrangThai == null)
                                 .OrderByDescending(k => k.NgayTao)
                                 .Take(8)
                                 .ToList();

            ViewBag.DanhSachDanhMuc = db.DanhMucKhoaHocs
                                       .Where(d => d.TrangThai == "Hoạt động")
                                       .OrderBy(d => d.ThuTu)
                                       .ToList();

            ViewBag.TongSoKhoaHoc = db.KhoaHocs.Count(k => k.TrangThai == "Hiển thị" || k.TrangThai == null);
            ViewBag.TongSoHocVien = db.HocViens.Count();
            ViewBag.TongSoGiangVien = db.GiangViens.Count();

            return View(dsKhoaHocMoi);
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