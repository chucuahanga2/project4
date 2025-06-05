using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class DoiMatKhauController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: DoiMatKhau
        public ActionResult Doimatkhau()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }

            ViewBag.TenDangNhap = Session["User"].ToString();
            return View();
        }
        [HttpPost]
        public ActionResult Doimatkhau(string TenDangNhap, string MatKhauCu, string MatKhauMoi, string XacNhanMatKhauMoi)
        {
            if (string.IsNullOrEmpty(TenDangNhap))
            {
                ViewBag.ThongBao = "Không xác định được tài khoản.";
                return View();
            }

            ViewBag.TenDangNhap = TenDangNhap; // Giữ lại để hiển thị sau khi submit

            var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == TenDangNhap);
            if (taiKhoan == null)
            {
                ViewBag.ThongBao = "Tài khoản không tồn tại.";
                return View();
            }

            if (taiKhoan.MatKhau != MatKhauCu)
            {
                ViewBag.ThongBao = "Mật khẩu cũ không chính xác.";
                return View();
            }

            if (MatKhauMoi.Length < 3 || MatKhauMoi.Length > 20)
            {
                ViewBag.ThongBao = "Mật khẩu mới phải từ 3 đến 20 ký tự.";
                return View();
            }

            if (MatKhauMoi != XacNhanMatKhauMoi)
            {
                ViewBag.ThongBao = "Mật khẩu xác nhận không chính xác.";
                return View();
            }

            // Cập nhật mật khẩu
            taiKhoan.MatKhau = MatKhauMoi;
            db.SaveChanges();

            ViewBag.ThongBao = "Đổi mật khẩu thành công!";
            return View();
        }
    }
}