using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("giangvien")]
    public class HomeGiangVienController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: HomeGiangVien
        public ActionResult Index()
        {
            if (Session["User"] == null) return RedirectToAction("DangNhap", "DangNhap");

            string username = Session["User"].ToString();
            var gv = db.GiangViens.FirstOrDefault(g => g.IDTenDangNhap == username);
            if (gv == null) return HttpNotFound("Không tìm thấy thông tin giảng viên.");

            var myCourses = db.KhoaHocs
                              .Include(k => k.DangKyKhoaHocs)
                              .Include(k => k.ChuongHocs.Select(c => c.BaiHocs))
                              .Where(k => k.IDGiangVien == gv.IDGiangVien)
                              .OrderByDescending(k => k.NgayTao)
                              .ToList();

            ViewBag.GiangVien = gv;
            ViewBag.TongSoKhoaHoc = myCourses.Count;
            ViewBag.TongSoHocVien = myCourses.Sum(k => k.DangKyKhoaHocs.Count);
            ViewBag.TongSoChuong = myCourses.Sum(k => k.ChuongHocs.Count);
            ViewBag.TongSoBaiHoc = myCourses.Sum(k => k.ChuongHocs.Sum(c => c.BaiHocs.Count));

            return View(myCourses);
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