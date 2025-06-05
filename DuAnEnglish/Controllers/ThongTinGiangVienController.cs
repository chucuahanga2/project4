using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;
namespace DuAnEnglish.Controllers
{
    public class ThongTinGiangVienController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: ThongTinGiangVien
        public ActionResult ThongTinGiangVien()
        {
            string tenDangNhap = Session["User"] as string;

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var giangVien = db.GiangViens
                            .Include("TaiKhoan")
                            .FirstOrDefault(hv => hv.IDTenDangNhap == tenDangNhap);

            if (giangVien == null)
            {
                return HttpNotFound("Không tìm thấy học viên.");
            }

            return View(giangVien); // Truyền model HocVien sang View
        }
        // POST: ThongGiangVien
        [HttpPost]
        public ActionResult ThongTinGiangVien(GiangVien model)
        {
            string tenDangNhap = Session["User"] as string;

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var giangVien = db.GiangViens.Include("TaiKhoan").FirstOrDefault(hv => hv.IDTenDangNhap == tenDangNhap);

            if (giangVien == null)
            {
                return HttpNotFound("Không tìm thấy giảng viên.");
            }

            // Kiểm tra nếu TaiKhoan là null
            if (giangVien.TaiKhoan == null)
            {
                ViewBag.ThongBao = "Không tìm thấy thông tin tài khoản giảng viên.";
                return View(giangVien);
            }

            // Kiểm tra các trường không được để trống
            if (string.IsNullOrWhiteSpace(model.TenGV))
            {
                ViewBag.ThongBao = "Tên giảng viên không được để trống.";
                return View(giangVien);
            }

            if (model.TenGV.Length < 5)
            {
                ViewBag.ThongBao = "Tên giảng viên phải có ít nhất 5 ký tự.";
                return View(giangVien);
            }

            if (string.IsNullOrWhiteSpace(model.DiaChi))
            {
                ViewBag.ThongBao = "Địa chỉ không được để trống.";
                return View(giangVien);
            }

            if (model.DiaChi.Length < 5)
            {
                ViewBag.ThongBao = "Địa chỉ phải có ít nhất 5 ký tự.";
                return View(giangVien);
            }

            if (model.NgaySinh == null)
            {
                ViewBag.ThongBao = "Ngày sinh không được để trống.";
                return View(giangVien);
            }

            if (string.IsNullOrWhiteSpace(model.GioiTinh))
            {
                ViewBag.ThongBao = "Giới tính không được để trống.";
                return View(giangVien);
            }

            // Kiểm tra Email và SDT chỉ khi TaiKhoan không phải là null
            if (model.TaiKhoan != null)
            {
                // Kiểm tra email
                if (string.IsNullOrEmpty(model.TaiKhoan.Email))
                {
                    ViewBag.ThongBao = "Email không được để trống.";
                    return View(giangVien);
                }

                var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com)$", RegexOptions.IgnoreCase);
                bool emailHopLe = emailRegex.IsMatch(model.TaiKhoan.Email);
                if (!emailHopLe)
                {
                    ViewBag.ThongBao = "Email không hợp lệ. Chỉ hỗ trợ email gmail, yahoo, và outlook.";
                    return View(giangVien);
                }

                // Kiểm tra số điện thoại
                if (string.IsNullOrEmpty(model.TaiKhoan.SDT))
                {
                    ViewBag.ThongBao = "Số điện thoại không được để trống.";
                    return View(giangVien);
                }

                var sdtRegex = new Regex(@"^\d{10}$");
                bool sdtHopLe = sdtRegex.IsMatch(model.TaiKhoan.SDT);
                if (!sdtHopLe)
                {
                    ViewBag.ThongBao = "Số điện thoại không hợp lệ. Phải có đúng 10 chữ số.";
                    return View(giangVien);
                }
            }


            // Cập nhật thông tin giảng viên
            giangVien.TenGV = model.TenGV;
            giangVien.NgaySinh = model.NgaySinh;
            giangVien.GioiTinh = model.GioiTinh;
            giangVien.DiaChi = model.DiaChi;

            // Lấy lại thông tin tài khoản từ cơ sở dữ liệu
            var taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.TenDangNhap == tenDangNhap);

            if (taiKhoan == null)
            {
                ViewBag.ThongBao = "Không tìm thấy tài khoản để cập nhật.";
                return View(giangVien);
            }

            // Cập nhật thông tin tài khoản
            taiKhoan.Email = model.TaiKhoan?.Email;
            taiKhoan.SDT = model.TaiKhoan?.SDT;
            //taiKhoan.Email = hocVien.TaiKhoan?.Email;  // Sử dụng toán tử ? để tránh lỗi nếu TaiKhoan là null
            //taiKhoan.SDT = hocVien.TaiKhoan?.SDT;      // Sử dụng toán tử ? để tránh lỗi nếu TaiKhoan là null
            db.SaveChanges(); // Lưu tất cả thay đổi vào cơ sở dữ liệu

            ViewBag.ThongBao = "Thông tin giảng viên đã được cập nhật thành công.";

            return View(giangVien); // Trả về thông tin đã được cập nhật
        }
    }
}