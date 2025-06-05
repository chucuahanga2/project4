using System;
using System.Linq;
using System.Web.Mvc;
using DuAnEnglish.Models;
using System.Data.Entity;
using System.Collections.Generic;

namespace DuAnEnglish.Controllers
{
    public class QuanLyHocVienController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        public ActionResult QuanLyHocVien(string idLop)
        {
            // Lấy tên đăng nhập từ session
            string tenDangNhap = Session["User"]?.ToString();
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để đăng ký khóa học";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            // Tìm giảng viên theo tên đăng nhập
            var giangVien = db.GiangViens.FirstOrDefault(gv => gv.IDTenDangNhap == tenDangNhap);
            if (giangVien == null)
            {
                TempData["ThongBao"] = "Không tìm thấy thông tin giảng viên.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách lớp do giảng viên quản lý
            var danhSachLop = db.LopHocs
                .Where(l => l.IDGiangVien == giangVien.IDGiangVien)
                .ToList();

            ViewBag.DanhSachLop = danhSachLop;
            ViewBag.IDLopDangChon = idLop;

            List<HocVien> hocViens;

            if (string.IsNullOrEmpty(idLop))
            {
                // Nếu không chọn lớp thì không hiển thị học viên nào
                hocViens = new List<HocVien>();
            }
            else
            {
                // Lấy học viên thuộc lớp đã chọn
                hocViens = db.HocVienLopHocs
                    .Where(hvl => hvl.IDLopHoc.Trim() == idLop.Trim())
                    .Select(hvl => hvl.HocVien)
                    .Include(hv => hv.TaiKhoan)
                    .Distinct()
                    .ToList();
            }

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(hocViens);
        }




        // GET: QuanLyHocVien/Them
        [HttpGet]
        public ActionResult Them(string idLop)
        {
            if (string.IsNullOrEmpty(idLop))
            {
                TempData["ThongBao"] = "Vui lòng chọn lớp học trước khi thêm học viên.";
                return RedirectToAction("QuanLyHocVien", new { idLop = "" });
            }

            ViewBag.IDLopHoc = idLop;

            // Lấy tất cả học viên (không lọc theo lớp)
            var tatCaHocVien = db.HocViens.ToList();
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            return View(tatCaHocVien);
        }

        // POST: QuanLyHocVien/Them
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemVaoLop(int idHocVien, string idLopHoc)
        {
            var daTonTai = db.HocVienLopHocs.Any(x => x.IDHocVien == idHocVien && x.IDLopHoc == idLopHoc);
            if (daTonTai)
            {
                ViewBag.IDLopHoc = idLopHoc;
                ViewBag.ThongBao = "Học viên đã tồn tại trong lớp này.";
                var danhSachHocVien = db.HocViens.ToList();
                return View("Them", danhSachHocVien);
            }

            // Nếu chưa có thì thêm vào lớp
            var hocVienLop = new HocVienLopHoc
            {
                IDHocVien = idHocVien,
                IDLopHoc = idLopHoc,
            };

            db.HocVienLopHocs.Add(hocVienLop);
            db.SaveChanges();

            TempData["ThongBao"] = "Thêm học viên thành công.";
            return RedirectToAction("Them", new { idLop = idLopHoc });
        }
        //Tim kiem
        [HttpPost]
        public ActionResult Them(string idLop, string tuKhoa)
        {
            ViewBag.IDLopHoc = idLop;

            var danhSachHocVien = db.HocViens.AsQueryable();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                // Chuyển từ khóa về dạng chuỗi để so sánh dễ dàng
                tuKhoa = tuKhoa.Trim();

                // Nếu từ khóa là số => tìm theo IDHocVien
                if (int.TryParse(tuKhoa, out int id))
                {
                    danhSachHocVien = danhSachHocVien.Where(hv =>
                        hv.IDHocVien.ToString().Contains(tuKhoa));
                }
                else
                {
                    // Nếu từ khóa không phải số => tìm theo IDTenDangNhap
                    danhSachHocVien = danhSachHocVien.Where(hv =>
                        hv.IDTenDangNhap.Contains(tuKhoa));
                }

                var ketQua = danhSachHocVien.ToList();

                if (ketQua.Any())
                {
                    ViewBag.ThongBao = "Đã tìm thấy " + ketQua.Count + " học viên.";
                    return View(ketQua);
                }
                else
                {
                    ViewBag.ThongBao = "Không tìm thấy học viên nào phù hợp.";
                    return View(new List<HocVien>());
                }
            }

            // Nếu không nhập từ khóa => load toàn bộ
            return View(db.HocViens.ToList());
        }

        // GET: QuanLyHocVien/Xoa
        public ActionResult Xoa(int id)
        {
            try
            {
                var hocVien = db.HocViens.Find(id);
                if (hocVien == null)
                {
                    TempData["ThongBao"] = "Không tìm thấy học viên.";
                    return RedirectToAction("QuanLyHocVien");
                }

                // Xóa điểm IELTS liên quan
                var ielts = db.DiemIELTS.Where(d => d.IDHocVien == id).ToList();
                db.DiemIELTS.RemoveRange(ielts);

                // Xóa điểm TOEIC liên quan
                var toeic = db.DiemTOEICs.Where(d => d.IDHocVien == id).ToList();
                db.DiemTOEICs.RemoveRange(toeic);

                // Xóa học viên lớp học liên quan
                var hvLop = db.HocVienLopHocs.Where(hv => hv.IDHocVien == id).ToList();
                db.HocVienLopHocs.RemoveRange(hvLop);


                // Xóa học viên
                db.HocViens.Remove(hocVien);

                // Lưu thay đổi
                db.SaveChanges();

                TempData["ThongBao"] = "Xóa học viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Lỗi khi xóa học viên: " + ex.Message;
            }

            return RedirectToAction("QuanLyHocVien");
        }

    }
}