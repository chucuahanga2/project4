using System;
using System.Linq;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class DangNhapController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: DangNhap/DangNhap
        public ActionResult DangNhap()
        {
            if (Session["User"] != null)
            {
                string role = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";
                if (role == "admin") return RedirectToAction("Index", "HomeAdmin");
                if (role == "giangvien") return RedirectToAction("Index", "HomeGiangVien");
                return RedirectToAction("Index", "HomeHocVien");
            }

            if (TempData["ThongBaoDangNhap"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBaoDangNhap"];
            }
            return View();
        }

        // POST: DangNhap/DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(string TenDangNhap, string MatKhau)
        {
            if (string.IsNullOrWhiteSpace(TenDangNhap) || string.IsNullOrWhiteSpace(MatKhau))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
                return View();
            }

            TenDangNhap = TenDangNhap.Trim();
            var user = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == TenDangNhap);

            if (user == null || !DuAnEnglish.Security.PasswordHelper.VerifyPassword(MatKhau, user.MatKhau))
            {
                ViewBag.ThongBao = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }

            // Tự động nâng cấp mật khẩu cũ dạng plain text sang mã băm SHA-256
            if (DuAnEnglish.Security.PasswordHelper.IsLegacyPassword(user.MatKhau))
            {
                try
                {
                    user.MatKhau = DuAnEnglish.Security.PasswordHelper.HashPassword(MatKhau);
                    db.SaveChanges();
                }
                catch { }
            }

            if (user.TrangThai == "Khóa" || (user.TrangThai != null && (user.TrangThai.IndexOf("khóa", StringComparison.OrdinalIgnoreCase) >= 0 || user.TrangThai.IndexOf("lock", StringComparison.OrdinalIgnoreCase) >= 0 || user.TrangThai.IndexOf("kha", StringComparison.OrdinalIgnoreCase) >= 0 || user.TrangThai.IndexOf("khã", StringComparison.OrdinalIgnoreCase) >= 0)))
            {
                ViewBag.ThongBao = "Tài khoản của bạn đã bị khóa! Vui lòng liên hệ ban quản trị.";
                return View();
            }

            // Lưu thông tin cốt lõi vào Session
            Session["User"] = user.TenDangNhap;
            string role = (user.LoaiTK ?? "").ToLower();
            Session["Role"] = role;

            if (role == "hocvien")
            {
                var hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == user.TenDangNhap);
                if (hocVien != null)
                {
                    Session["IDHocVien"] = hocVien.IDHocVien;
                    Session["TenHienThi"] = string.IsNullOrEmpty(hocVien.TenHV) ? user.TenDangNhap : hocVien.TenHV;
                }
                else
                {
                    Session["TenHienThi"] = user.TenDangNhap;
                }
                return RedirectToAction("Index", "HomeHocVien");
            }
            else if (role == "giangvien")
            {
                var giangVien = db.GiangViens.FirstOrDefault(g => g.IDTenDangNhap == user.TenDangNhap);
                if (giangVien != null)
                {
                    Session["IDGiangVien"] = giangVien.IDGiangVien;
                    Session["TenHienThi"] = string.IsNullOrEmpty(giangVien.TenGV) ? user.TenDangNhap : giangVien.TenGV;
                }
                else
                {
                    Session["TenHienThi"] = user.TenDangNhap;
                }
                return RedirectToAction("Index", "HomeGiangVien");
            }
            else if (role == "admin")
            {
                Session["TenHienThi"] = "Quản trị viên";
                return RedirectToAction("Index", "HomeAdmin");
            }
            else
            {
                ViewBag.ThongBao = "Loại tài khoản không hợp lệ!";
                return View();
            }
        }

        // GET: DangNhap/DangXuat
        public ActionResult DangXuat()
        {
            Session.Clear();
            Session.Abandon();
            TempData["ThongBaoDangNhap"] = "Bạn đã đăng xuất thành công.";
            return RedirectToAction("DangNhap");
        }

        // Chuyển hướng sang trang Đăng Ký
        public ActionResult DangKy()
        {
            return RedirectToAction("DangKy", "DangKy");
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