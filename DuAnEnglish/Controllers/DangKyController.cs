using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class DangKyController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: DangKy
        public ActionResult DangKy()
        {
            if (Session["User"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(string TenDangNhap, string MatKhau, string NhapLaiMatKhau, string Email, string SDT, string HoTen)
        {
            TenDangNhap = (TenDangNhap ?? "").Trim();
            HoTen = (HoTen ?? "").Trim();
            Email = (Email ?? "").Trim();
            SDT = (SDT ?? "").Trim();

            if (string.IsNullOrWhiteSpace(TenDangNhap) || string.IsNullOrWhiteSpace(MatKhau) || string.IsNullOrWhiteSpace(HoTen))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ các trường bắt buộc!";
                return View();
            }

            if (TenDangNhap.Length < 4 || TenDangNhap.Contains(" "))
            {
                ViewBag.ThongBao = "Tên đăng nhập phải có ít nhất 4 ký tự và không chứa khoảng trắng!";
                return View();
            }

            if (HoTen.Length < 3)
            {
                ViewBag.ThongBao = "Họ tên phải có ít nhất 3 ký tự!";
                return View();
            }

            if (MatKhau.Length < 3 || MatKhau.Length > 20)
            {
                ViewBag.ThongBao = "Mật khẩu phải có độ dài từ 3 đến 20 ký tự!";
                return View();
            }

            if (MatKhau != NhapLaiMatKhau)
            {
                ViewBag.ThongBao = "Mật khẩu và xác nhận mật khẩu không khớp!";
                return View();
            }

            // Kiểm tra trùng lặp
            if (db.TaiKhoans.Any(t => t.TenDangNhap.ToLower() == TenDangNhap.ToLower()))
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác!";
                return View();
            }

            // Kiểm tra email
            if (!string.IsNullOrEmpty(Email))
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(Email))
                {
                    ViewBag.ThongBao = "Định dạng email không hợp lệ!";
                    return View();
                }
            }

            // Kiểm tra số điện thoại
            if (!string.IsNullOrEmpty(SDT))
            {
                var sdtRegex = new Regex(@"^\d{9,11}$");
                if (!sdtRegex.IsMatch(SDT))
                {
                    ViewBag.ThongBao = "Số điện thoại phải từ 9 đến 11 chữ số!";
                    return View();
                }
            }

            try
            {
                var taiKhoanMoi = new TaiKhoan
                {
                    TenDangNhap = TenDangNhap,
                    MatKhau = MatKhau,
                    Email = string.IsNullOrEmpty(Email) ? null : Email,
                    SDT = string.IsNullOrEmpty(SDT) ? null : SDT,
                    LoaiTK = "hocvien",
                    TrangThai = "Hoạt động"
                };
                db.TaiKhoans.Add(taiKhoanMoi);

                var hocVienMoi = new HocVien
                {
                    IDTenDangNhap = TenDangNhap,
                    TenHV = HoTen,
                    NgaySinh = null,
                    GioiTinh = null,
                    DiaChi = null
                };
                db.HocViens.Add(hocVienMoi);

                db.SaveChanges();

                TempData["ThongBaoDangNhap"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap", "DangNhap");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = "Lỗi khi đăng ký tài khoản: " + ex.Message;
                return View();
            }
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