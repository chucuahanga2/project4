using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("admin")]
    public class QuanLyGiangVienController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyGiangVien
        public ActionResult QuanLyGiangVien(string search = "")
        {
            var query = db.GiangViens
                          .Include(g => g.TaiKhoan)
                          .Include(g => g.KhoaHocs)
                          .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(gv => gv.IDGiangVien.ToString().Contains(kw) ||
                                          (gv.TenGV != null && gv.TenGV.ToLower().Contains(kw)) ||
                                          (gv.ChuyenMon != null && gv.ChuyenMon.ToLower().Contains(kw)) ||
                                          (gv.TaiKhoan != null && gv.TaiKhoan.Email != null && gv.TaiKhoan.Email.ToLower().Contains(kw)));
            }

            var dsGiangVien = query.OrderByDescending(gv => gv.IDGiangVien).ToList();

            ViewBag.CurrentSearch = search;
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(dsGiangVien);
        }

        // GET: QuanLyGiangVien/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuanLyGiangVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GiangVien gv, string TenDangNhap, string MatKhau, string Email, string SDT)
        {
            if (string.IsNullOrWhiteSpace(TenDangNhap) || string.IsNullOrWhiteSpace(MatKhau) || string.IsNullOrWhiteSpace(gv.TenGV))
            {
                ViewBag.ThongBao = "Vui lòng nhập tên đăng nhập, mật khẩu và họ tên giảng viên!";
                return View(gv);
            }

            TenDangNhap = TenDangNhap.Trim();
            if (db.TaiKhoans.Any(t => t.TenDangNhap.ToLower() == TenDangNhap.ToLower()))
            {
                ViewBag.ThongBao = "Tên đăng nhập này đã tồn tại!";
                return View(gv);
            }

            // Tạo tài khoản giảng viên được mã hóa mật khẩu chuẩn PBKDF2
            var tk = new TaiKhoan
            {
                TenDangNhap = TenDangNhap,
                MatKhau = PasswordHelper.HashPassword(MatKhau),
                Email = Email,
                SDT = SDT,
                LoaiTK = "giangvien",
                TrangThai = "Hoạt động"
            };
            db.TaiKhoans.Add(tk);

            // Tạo hồ sơ giảng viên
            gv.IDTenDangNhap = TenDangNhap;
            db.GiangViens.Add(gv);

            db.SaveChanges();

            TempData["ThongBao"] = "Thêm giảng viên mới thành công!";
            return RedirectToAction("QuanLyGiangVien");
        }

        // GET: QuanLyGiangVien/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var gv = db.GiangViens.Include(g => g.TaiKhoan).Include(g => g.KhoaHocs).FirstOrDefault(g => g.IDGiangVien == id);
            if (gv == null) return HttpNotFound();

            return View(gv);
        }

        // POST: QuanLyGiangVien/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(GiangVien gv, string Email, string SDT, string TrangThai)
        {
            if (string.IsNullOrWhiteSpace(gv.TenGV))
            {
                ViewBag.ThongBao = "Tên giảng viên không được để trống!";
                return View(gv);
            }

            var existing = db.GiangViens.Include(g => g.TaiKhoan).FirstOrDefault(g => g.IDGiangVien == gv.IDGiangVien);
            if (existing == null) return HttpNotFound();

            existing.TenGV = gv.TenGV;
            existing.NgaySinh = gv.NgaySinh;
            existing.GioiTinh = gv.GioiTinh;
            existing.DiaChi = gv.DiaChi;
            existing.ChuyenMon = gv.ChuyenMon;
            existing.BangCap = gv.BangCap;

            if (existing.TaiKhoan != null)
            {
                existing.TaiKhoan.Email = Email;
                existing.TaiKhoan.SDT = SDT;
                if (!string.IsNullOrEmpty(TrangThai)) existing.TaiKhoan.TrangThai = TrangThai;
            }

            db.SaveChanges();
            TempData["ThongBao"] = "Cập nhật thông tin giảng viên thành công!";
            return RedirectToAction("QuanLyGiangVien");
        }

        // POST: Khóa / Mở khóa giảng viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KhoaMoTaiKhoan(int id)
        {
            var gv = db.GiangViens.Include(g => g.TaiKhoan).FirstOrDefault(g => g.IDGiangVien == id);
            if (gv == null || gv.TaiKhoan == null)
            {
                TempData["ThongBao"] = "Không tìm thấy giảng viên!";
                return RedirectToAction("QuanLyGiangVien");
            }

            if (gv.TaiKhoan.TrangThai == "Khóa")
            {
                gv.TaiKhoan.TrangThai = "Hoạt động";
                TempData["ThongBao"] = string.Format("Đã mở khóa tài khoản giảng viên '{0}'!", gv.TenGV);
            }
            else
            {
                gv.TaiKhoan.TrangThai = "Khóa";
                TempData["ThongBao"] = string.Format("Đã khóa tài khoản giảng viên '{0}'!", gv.TenGV);
            }

            db.SaveChanges();
            return RedirectToAction("QuanLyGiangVien");
        }

        // POST: Xóa giảng viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var gv = db.GiangViens.Include(g => g.KhoaHocs).Include(g => g.TaiKhoan).FirstOrDefault(g => g.IDGiangVien == id);
            if (gv == null) return HttpNotFound();

            if (gv.KhoaHocs != null && gv.KhoaHocs.Count > 0)
            {
                // Có khóa học phụ trách -> chuyển sang khóa tài khoản
                if (gv.TaiKhoan != null) gv.TaiKhoan.TrangThai = "Khóa";
                db.SaveChanges();
                TempData["ThongBao"] = string.Format("Giảng viên đang phụ trách {0} khóa học. Đã tự động chuyển trạng thái tài khoản sang 'Khóa' thay vì xóa vật lý!", gv.KhoaHocs.Count);
                return RedirectToAction("QuanLyGiangVien");
            }

            db.GiangViens.Remove(gv);
            if (gv.TaiKhoan != null) db.TaiKhoans.Remove(gv.TaiKhoan);
            db.SaveChanges();

            TempData["ThongBao"] = "Xóa giảng viên thành công!";
            return RedirectToAction("QuanLyGiangVien");
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
