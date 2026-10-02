using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("admin")]
    public class QuanLyHocVienController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyHocVien
        public ActionResult QuanLyHocVien(string search = "")
        {
            var query = db.HocViens.Include(h => h.TaiKhoan).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(h => h.IDHocVien.ToString().Contains(kw) ||
                                        (h.TenHV != null && h.TenHV.ToLower().Contains(kw)) ||
                                        (h.IDTenDangNhap != null && h.IDTenDangNhap.ToLower().Contains(kw)) ||
                                        (h.TaiKhoan != null && h.TaiKhoan.Email != null && h.TaiKhoan.Email.ToLower().Contains(kw)));
            }

            var dsHocVien = query.OrderByDescending(h => h.IDHocVien).ToList();

            ViewBag.CurrentSearch = search;
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(dsHocVien);
        }

        // GET: QuanLyHocVien/ChiTiet/5
        public ActionResult ChiTiet(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var hocVien = db.HocViens
                            .Include(h => h.TaiKhoan)
                            .Include(h => h.DangKyKhoaHocs.Select(d => d.KhoaHoc))
                            .FirstOrDefault(h => h.IDHocVien == id);

            if (hocVien == null) return HttpNotFound();

            return View(hocVien);
        }

        // POST: Khóa hoặc Mở khóa tài khoản học viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KhoaMoTaiKhoan(int id)
        {
            var hocVien = db.HocViens.Include(h => h.TaiKhoan).FirstOrDefault(h => h.IDHocVien == id);
            if (hocVien == null || hocVien.TaiKhoan == null)
            {
                TempData["ThongBao"] = "Không tìm thấy tài khoản học viên!";
                return RedirectToAction("QuanLyHocVien");
            }

            if (hocVien.TaiKhoan.TrangThai == "Khóa")
            {
                hocVien.TaiKhoan.TrangThai = "Hoạt động";
                TempData["ThongBao"] = string.Format("Đã mở khóa thành công tài khoản học viên '{0}'!", hocVien.TenHV ?? hocVien.IDTenDangNhap);
            }
            else
            {
                hocVien.TaiKhoan.TrangThai = "Khóa";
                TempData["ThongBao"] = string.Format("Đã khóa tài khoản học viên '{0}'!", hocVien.TenHV ?? hocVien.IDTenDangNhap);
            }

            db.SaveChanges();
            return RedirectToAction("QuanLyHocVien");
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