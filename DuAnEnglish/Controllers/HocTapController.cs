using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class HocTapController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: HocTap/XemLop
        public ActionResult XemLop()
        {
            // Lấy tên đăng nhập của người dùng từ session
            string tenDangNhap = Session["User"] as string;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
                return RedirectToAction("DangNhap", "DangNhap");
            }

            // Lấy thông tin học viên từ bảng HocVien theo tên đăng nhập
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.IDTenDangNhap == tenDangNhap);
            if (hocVien == null)
            {
                return HttpNotFound("Không tìm thấy học viên.");
            }

            // Lấy danh sách các lớp học mà học viên đã đăng ký
            var lopHocs = db.HocVienLopHocs
                             .Where(hvlh => hvlh.IDHocVien == hocVien.IDHocVien)
                             .Select(hvlh => hvlh.LopHoc)
                             .Include(lh => lh.PhongHoc)
                             .ToList();

            // Truyền mã học viên vào ViewBag để sử dụng trong view
            ViewBag.MahocVien = hocVien.IDHocVien;

            // Trả về View với danh sách lớp học
            return View(lopHocs);
        }
        // GET: hiển thị điểm số lên
        public ActionResult XemDiem(string idLopHoc, int idHocVien)
        {
            // Kiểm tra lớp học có tồn tại không
            var lopHoc = db.LopHocs.FirstOrDefault(lh => lh.IDLopHoc == idLopHoc);
            if (lopHoc == null)
            {
                return HttpNotFound("Không tìm thấy lớp học.");
            }

            // Lấy khóa học tương ứng với lớp
            var khoaHoc = db.KhoaHocs.FirstOrDefault(kh => kh.IDKhoaHoc == lopHoc.IDKhoaHoc);
            if (khoaHoc == null)
            {
                return HttpNotFound("Không tìm thấy khóa học.");
            }

            // Lấy thông tin học viên
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.IDHocVien == idHocVien);
            if (hocVien == null)
            {
                return HttpNotFound("Không tìm thấy học viên.");
            }

            // Truyền thông tin phụ vào ViewBag
            ViewBag.IDLopHoc = idLopHoc;
            ViewBag.TenLopHoc = lopHoc.TenLop;
            ViewBag.IDHocVien = idHocVien;
            ViewBag.TenHocVien = hocVien.TenHV;

            // Kiểm tra danh mục khóa học và lấy điểm theo loại khóa học
            var danhMuc = khoaHoc.DanhMuc?.Trim().ToLower();
            if (danhMuc == "ielts")
            {
                var diem = db.DiemIELTS.FirstOrDefault(d => d.IDHocVien == idHocVien && d.IDLopHoc == idLopHoc);
                if (diem == null)
                {
                    diem = new DiemIELT
                    {
                        DiemNghe = null,
                        DiemNoi = null,
                        DiemDoc = null,
                        DiemViet = null,
                        TongDiem = null
                    };
                }

                ViewBag.LoaiKhoaHoc = "IELTS";
                return View("XemDiemIELTS", diem);
            }
            else if (danhMuc == "toeic")
            {
                var diem = db.DiemTOEICs.FirstOrDefault(d => d.IDHocVien == idHocVien && d.IDLopHoc == idLopHoc);
                if (diem == null)
                {
                    diem = new DiemTOEIC
                    {
                        Part1 = null,
                        Part2 = null,
                        Part3 = null,
                        Part4 = null,
                        DiemNghe = null,
                        Part5 = null,
                        Part6 = null,
                        Part7 = null,
                        DiemDoc = null,
                        DiemNoi = null,
                        DiemViet = null,
                        TongDiem = null
                    };
                }

                ViewBag.LoaiKhoaHoc = "TOEIC";
                return View("XemDiemTOEIC", diem);
            }
            else
            {
                return Content("Khóa học không thuộc IELTS hay TOEIC.");
            }
        }



    }
}