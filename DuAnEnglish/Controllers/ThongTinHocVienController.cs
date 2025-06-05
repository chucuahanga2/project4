using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class ThongTinHocVienController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: ThongTinHocVien
        public ActionResult ThongTinHocVien()
        {
            string tenDangNhap = Session["User"] as string;

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var hocVien = db.HocViens
                            .Include("TaiKhoan")
                            .FirstOrDefault(hv => hv.IDTenDangNhap == tenDangNhap);

            if (hocVien == null)
            {
                return HttpNotFound("Không tìm thấy học viên.");
            }

            return View(hocVien); // Truyền model HocVien sang View
        }

        // POST: ThongTinHocVien
        [HttpPost]
        public ActionResult ThongTinHocVien(HocVien model)
        {
            string tenDangNhap = Session["User"] as string;

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var hocVien = db.HocViens.Include("TaiKhoan").FirstOrDefault(hv => hv.IDTenDangNhap == tenDangNhap);

            if (hocVien == null)
            {
                return HttpNotFound("Không tìm thấy học viên.");
            }

            // Kiểm tra nếu TaiKhoan là null
            if (hocVien.TaiKhoan == null)
            {
                ViewBag.ThongBao = "Không tìm thấy thông tin tài khoản học viên.";
                return View(hocVien);
            }

            // Kiểm tra các trường không được để trống
            if (string.IsNullOrWhiteSpace(model.TenHV))
            {
                ViewBag.ThongBao = "Tên học viên không được để trống.";
                return View(hocVien);
            }

            if (model.TenHV.Length < 5)
            {
                ViewBag.ThongBao = "Tên học viên phải có ít nhất 5 ký tự.";
                return View(hocVien);
            }

            if (string.IsNullOrWhiteSpace(model.DiaChi))
            {
                ViewBag.ThongBao = "Địa chỉ không được để trống.";
                return View(hocVien);
            }

            if (model.DiaChi.Length < 5)
            {
                ViewBag.ThongBao = "Địa chỉ phải có ít nhất 5 ký tự.";
                return View(hocVien);
            }

            if (model.NgaySinh == null)
            {
                ViewBag.ThongBao = "Ngày sinh không được để trống.";
                return View(hocVien);
            }

            if (string.IsNullOrWhiteSpace(model.GioiTinh))
            {
                ViewBag.ThongBao = "Giới tính không được để trống.";
                return View(hocVien);
            }

            // Kiểm tra Email và SDT chỉ khi TaiKhoan không phải là null
            if (model.TaiKhoan != null)
            {
                // Kiểm tra email
                if (string.IsNullOrEmpty(model.TaiKhoan.Email))
                {
                    ViewBag.ThongBao = "Email không được để trống.";
                    return View(hocVien);
                }

                var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com)$", RegexOptions.IgnoreCase);
                bool emailHopLe = emailRegex.IsMatch(model.TaiKhoan.Email);
                if (!emailHopLe)
                {
                    ViewBag.ThongBao = "Email không hợp lệ. Chỉ hỗ trợ email gmail, yahoo, và outlook.";
                    return View(hocVien);
                }

                // Kiểm tra số điện thoại
                if (string.IsNullOrEmpty(model.TaiKhoan.SDT))
                {
                    ViewBag.ThongBao = "Số điện thoại không được để trống.";
                    return View(hocVien);
                }

                var sdtRegex = new Regex(@"^\d{10}$");
                bool sdtHopLe = sdtRegex.IsMatch(model.TaiKhoan.SDT);
                if (!sdtHopLe)
                {
                    ViewBag.ThongBao = "Số điện thoại không hợp lệ. Phải có đúng 10 chữ số.";
                    return View(hocVien);
                }
            }


            // Cập nhật thông tin học viên
            hocVien.TenHV = model.TenHV;
            hocVien.NgaySinh = model.NgaySinh;
            hocVien.GioiTinh = model.GioiTinh;
            hocVien.DiaChi = model.DiaChi;

            // Lấy lại thông tin tài khoản từ cơ sở dữ liệu
            var taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.TenDangNhap == tenDangNhap);

            if (taiKhoan == null)
            {
                ViewBag.ThongBao = "Không tìm thấy tài khoản để cập nhật.";
                return View(hocVien);
            }

            // Cập nhật thông tin tài khoản
            taiKhoan.Email = model.TaiKhoan?.Email;
            taiKhoan.SDT = model.TaiKhoan?.SDT;
            //taiKhoan.Email = hocVien.TaiKhoan?.Email;  // Sử dụng toán tử ? để tránh lỗi nếu TaiKhoan là null
            //taiKhoan.SDT = hocVien.TaiKhoan?.SDT;      // Sử dụng toán tử ? để tránh lỗi nếu TaiKhoan là null
            db.SaveChanges(); // Lưu tất cả thay đổi vào cơ sở dữ liệu

            ViewBag.ThongBao = "Thông tin học viên đã được cập nhật thành công.";

            return View(hocVien); // Trả về thông tin đã được cập nhật
        }


        //[HttpPost]
        //public ActionResult ThongTinHocVien(HocVien model)
        //{
        //    string tenDangNhap = Session["User"] as string;

        //    if (string.IsNullOrEmpty(tenDangNhap))
        //    {
        //        return RedirectToAction("DangNhap", "DangNhap");
        //    }

        //    var hocVien = db.HocViens.Include("TaiKhoan").FirstOrDefault(hv => hv.IDTenDangNhap == tenDangNhap);

        //    if (hocVien == null)
        //    {
        //        return HttpNotFound("Không tìm thấy học viên.");
        //    }

        //    // Debugging: Kiểm tra nếu TaiKhoan là null
        //    if (hocVien.TaiKhoan == null)
        //    {
        //        ViewBag.ThongBao = "Không tìm thấy thông tin tài khoản học viên.";
        //        return View(hocVien);
        //    }

        //    // Debugging: In ra thông tin Email và SDT của TaiKhoan
        //    //System.Diagnostics.Debug.WriteLine("Email: " + hocVien.TaiKhoan.Email);
        //    //System.Diagnostics.Debug.WriteLine("Số điện thoại: " + hocVien.TaiKhoan.SDT);

        //    // Kiểm tra các trường không được để trống
        //    if (string.IsNullOrWhiteSpace(model.TenHV))
        //    {
        //        ViewBag.ThongBao = "Tên học viên không được để trống.";
        //        return View(hocVien);
        //    }

        //    if (model.TenHV.Length < 5)
        //    {
        //        ViewBag.ThongBao = "Tên học viên phải có ít nhất 5 ký tự.";
        //        return View(hocVien);
        //    }

        //    if (string.IsNullOrWhiteSpace(model.DiaChi))
        //    {
        //        ViewBag.ThongBao = "Địa chỉ không được để trống.";
        //        return View(hocVien);
        //    }

        //    if (model.DiaChi.Length < 5)
        //    {
        //        ViewBag.ThongBao = "Địa chỉ phải có ít nhất 5 ký tự.";
        //        return View(hocVien);
        //    }

        //    if (model.NgaySinh == null)
        //    {
        //        ViewBag.ThongBao = "Ngày sinh không được để trống.";
        //        return View(hocVien);
        //    }

        //    if (string.IsNullOrWhiteSpace(model.GioiTinh))
        //    {
        //        ViewBag.ThongBao = "Giới tính không được để trống.";
        //        return View(hocVien);
        //    }

        //    // Kiểm tra email
        //    if (hocVien.TaiKhoan.Email == null)
        //    {
        //        ViewBag.ThongBao = "Email không được để trống.";
        //        return View(hocVien);
        //    }

        //    var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com)$", RegexOptions.IgnoreCase);
        //    bool emailHopLe = emailRegex.IsMatch(hocVien.TaiKhoan.Email);
        //    if (!emailHopLe)
        //    {
        //        ViewBag.ThongBao = "Email không hợp lệ. Chỉ hỗ trợ email gmail, yahoo, và outlook.";
        //        return View(hocVien);
        //    }

        //    // Kiểm tra số điện thoại
        //    if (hocVien.TaiKhoan.SDT == null)
        //    {
        //        ViewBag.ThongBao = "Số điện thoại không được để trống.";
        //        return View(hocVien);
        //    }

        //    var sdtRegex = new Regex(@"^\d{10}$");
        //    bool sdtHopLe = sdtRegex.IsMatch(hocVien.TaiKhoan.SDT);
        //    if (!sdtHopLe)
        //    {
        //        ViewBag.ThongBao = "Số điện thoại không hợp lệ. Phải có đúng 10 chữ số.";
        //        return View(hocVien);
        //    }

        //    // Cập nhật thông tin học viên
        //    hocVien.TenHV = model.TenHV;
        //    hocVien.NgaySinh = model.NgaySinh;
        //    hocVien.GioiTinh = model.GioiTinh;
        //    hocVien.DiaChi = model.DiaChi;

        //    // Lấy lại thông tin tài khoản từ cơ sở dữ liệu
        //    var taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.TenDangNhap == tenDangNhap);

        //    if (taiKhoan == null)
        //    {
        //        ViewBag.ThongBao = "Không tìm thấy tài khoản để cập nhật.";
        //        return View(hocVien);
        //    }

        //    // Cập nhật thông tin tài khoản
        //    taiKhoan.Email = hocVien.TaiKhoan.Email;
        //    taiKhoan.SDT = hocVien.TaiKhoan.SDT;
        //    db.SaveChanges();

        //    db.SaveChanges(); // Lưu tất cả thay đổi vào cơ sở dữ liệu

        //    ViewBag.ThongBao = "Thông tin học viên đã được cập nhật thành công.";

        //    return View(hocVien); // Trả về thông tin đã được cập nhật
        //}

    }
}