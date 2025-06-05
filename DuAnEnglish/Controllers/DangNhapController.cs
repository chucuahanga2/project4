using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class DangNhapController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: TaiKhoans/DangNhap
        // Dùng để hiển thị trang đăng nhập
        public ActionResult DangNhap()
        {
            if (TempData["ThongBaoDangNhap"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBaoDangNhap"];
            }
            return View();
        }

        // POST: TaiKhoans/DangNhap
        [HttpPost]
        public ActionResult DangNhap(string TenDangNhap, string MatKhau)
        {
            // Kiểm tra có để trống tên đăng nhập hoặc mật khẩu không
            if (string.IsNullOrEmpty(TenDangNhap) || string.IsNullOrEmpty(MatKhau))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
                return View();
            }

            var user = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == TenDangNhap);

            if (user == null || user.MatKhau != MatKhau)
            {
                ViewBag.ThongBao = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }

            if (user.TrangThai == "Khóa") // Kiểm tra trạng thái tài khoản
            {
                ViewBag.ThongBao = "Tài khoản của bạn đã bị khóa!";
                return View();
            }

            // Đăng nhập thành công, lưu thông tin vào session
            Session["User"] = user.TenDangNhap;
            Session["Role"] = user.LoaiTK;


            // Kiểm tra phân quyền và chuyển hướng đến trang Home tương ứng
            switch (user.LoaiTK)
            {
                case "admin": // Admin
                    return RedirectToAction("Index", "HomeAdmin"); // Redirect đến trang Home của Admin
                case "giangvien": // Giảng viên
                    return RedirectToAction("Index", "HomeGiangVien"); // Redirect đến trang Home của Giảng viên
                case "hocvien": // Học viên
                    return RedirectToAction("Index", "HomeHocVien"); // Redirect đến trang Home của Học viên
                default:
                    ViewBag.ThongBao = "Phân quyền không hợp lệ!";
                    return View();
            }
        }

        // Đăng xuất
        public ActionResult DangXuat()
        {
            Session.Clear(); // Xóa tất cả session
            Session.Abandon(); // Hủy session hiện tại

            // Kiểm tra xem session đã bị xóa chưa
            //if (Session["User"] == null && Session["Role"] == null)
            //{
            //    System.Diagnostics.Debug.WriteLine( "Session đã được xóa thành công!");
            //}
            //else
            //{
            //    ViewBag.DebugMessage = "Lỗi: Session chưa bị xóa!";
            //}
            return View("DangNhap"); // Quay lại trang đăng nhập
        }

        // Chuyển hướng sang trang Đăng Ký
        public ActionResult DangKy()
        {
            return RedirectToAction("DangKy", "DangKy");
        }
    }
}