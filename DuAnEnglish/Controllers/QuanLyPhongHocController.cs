using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("admin")]
    public class QuanLyPhongHocController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: QuanLyPhongHoc
        public ActionResult QuanLyPhongHoc()
        {
            string tenDangNhap = Session["User"] as string;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }
            var danhSachPhongHoc = db.PhongHocs.ToList();
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            return View(danhSachPhongHoc);
        }
        //GET
        public ActionResult Them()
        {
            return View();
        }
        // POST: Them Phong Hoc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them(PhongHoc phongHoc)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra mã phòng học đã tồn tại chưa
                var existingPhongHoc = db.PhongHocs.FirstOrDefault(p => p.IDPhongHoc == phongHoc.IDPhongHoc);
                if (existingPhongHoc != null)
                {
                    ViewBag.ThongBao = "Mã phòng học đã tồn tại.";
                    return View();
                }

                // Kiểm tra sức chứa hợp lệ
                if (phongHoc.SucChua < 1 )
                {
                    ViewBag.ThongBao = "Sức chứa phải lớn hơn 0";
                    return View();
                }

                // Thêm phòng học vào database
                db.PhongHocs.Add(phongHoc);
                db.SaveChanges();

                TempData["ThongBao"] = "Thêm phòng học thành công!";
                return RedirectToAction("QuanLyPhongHoc");
            }

            return View();
        }

        public ActionResult Xem(int id)
        {
            string tenDangNhap = Session["User"] as string;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var phonghoc = db.PhongHocs.FirstOrDefault(p => p.IDPhongHoc == id);
            if (phonghoc == null)
            {
                TempData["ThongBao"] = "Phòng học không tồn tại";
                return RedirectToAction("QuanLyPhongHoc");
            }

            return View(phonghoc); // Truyền model vào view
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhat(PhongHoc phongHoc)
        {
            if (string.IsNullOrWhiteSpace(phongHoc.TenPhong))
            {
                ViewBag.ThongBao = "Vui lòng nhập tên phòng học.";
                return View("Xem", phongHoc);
            }

            if (phongHoc.SucChua < 1)
            {
                ViewBag.ThongBao = "Sức chứa phải lớn hơn 0";
                return View("Xem", phongHoc);
            }
            if (ModelState.IsValid)
            {
                var existing = db.PhongHocs.FirstOrDefault(p => p.IDPhongHoc == phongHoc.IDPhongHoc);
                if (existing != null)
                {
                    existing.TenPhong = phongHoc.TenPhong;
                    existing.SucChua = phongHoc.SucChua;
                    db.SaveChanges();
                    TempData["ThongBao"] = "Cập nhật phòng học thành công!";
                    return RedirectToAction("QuanLyPhongHoc");
                }
                else
                {
                    ViewBag.ThongBao = "Không tìm thấy phòng học.";
                    return View("Xem", phongHoc);
                }
            }

            ViewBag.ThongBao = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
            return View("Xem", phongHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult xoa(int id)
        {
            string tenDangNhap = Session["User"] as string;

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var phonghoc = db.PhongHocs.FirstOrDefault(ph => ph.IDPhongHoc == id);

            if (phonghoc == null)
            {
                TempData["ThongBao"] = "Phòng học không tồn tại";
                return RedirectToAction("QuanLyPhongHoc");
            }

            // Tìm tất cả lớp học đang dùng phòng này và set IDPhongHoc = null
            var lopHocsLienQuan = db.LopHocs.Where(lh => lh.IDPhongHoc == id).ToList();
            foreach (var lop in lopHocsLienQuan)
            {
                lop.IDPhongHoc = null;
            }

            // Lưu thay đổi cập nhật khóa ngoại
            db.SaveChanges();

            // Sau đó mới xóa phòng học
            db.PhongHocs.Remove(phonghoc);
            db.SaveChanges();

            TempData["ThongBao"] = "Xóa phòng học thành công";
            return RedirectToAction("QuanLyPhongHoc");
        }

    }
}