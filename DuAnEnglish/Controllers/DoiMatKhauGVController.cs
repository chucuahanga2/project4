using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class DoiMatKhauGVController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: DoiMatKhau
        public ActionResult DoiMatKhauGV()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }

            ViewBag.TenDangNhap = Session["User"].ToString();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhauGV(string TenDangNhap, string MatKhauCu, string MatKhauMoi, string XacNhanMatKhauMoi)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string currentUser = Session["User"].ToString();
            if (string.IsNullOrEmpty(TenDangNhap) || currentUser != TenDangNhap)
            {
                ViewBag.ThongBao = "Không có quyền đổi mật khẩu cho tài khoản này.";
                return View();
            }

            ViewBag.TenDangNhap = TenDangNhap;

            var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == TenDangNhap);
            if (taiKhoan == null)
            {
                ViewBag.ThongBao = "Tài khoản không tồn tại.";
                return View();
            }

            if (!DuAnEnglish.Security.PasswordHelper.VerifyPassword(MatKhauCu, taiKhoan.MatKhau))
            {
                ViewBag.ThongBao = "Mật khẩu cũ không chính xác.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(MatKhauMoi) || MatKhauMoi.Length < 3 || MatKhauMoi.Length > 20)
            {
                ViewBag.ThongBao = "Mật khẩu mới phải từ 3 đến 20 ký tự.";
                return View();
            }

            if (MatKhauMoi != XacNhanMatKhauMoi)
            {
                ViewBag.ThongBao = "Mật khẩu xác nhận không chính xác.";
                return View();
            }

            // Cập nhật mật khẩu băm
            taiKhoan.MatKhau = DuAnEnglish.Security.PasswordHelper.HashPassword(MatKhauMoi);
            db.SaveChanges();

            ViewBag.ThongBao = "Đổi mật khẩu thành công!";
            return View();
        }
    }
}