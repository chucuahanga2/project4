using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class QuanLyGiangVienController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: QuanLyGiangVien
        public ActionResult QuanLyGiangVien(string search)
        {
            // Lọc danh sách tài khoản (nếu loai == null thì hiển thị tất cả)
            var danhSach = db.GiangViens.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                danhSach = danhSach.Where(gv =>
                    gv.IDGiangVien.ToString().Contains(search) ||  // Tìm theo mã
                    gv.TenGV.Contains(search));                    // Tìm theo tên
            }
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            return View(danhSach.ToList());
        }

        // POST: QuanLyGiangVien/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(GiangVien giangVien)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.GiangViens.Add(giangVien);
        //        db.SaveChanges();
        //        TempData["ThongBao"] = "Thêm giảng viên thành công!";
        //        return RedirectToAction("QuanLyGiangVien");
        //    }
        //    return View(giangVien);  // Truyền lại giảng viên nếu có lỗi
        //}
        // GET: Delete
        public ActionResult Delete(int id)
        {
            if (id == 0)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            GiangVien gv = db.GiangViens.Find(id);
            if (gv == null)
                return HttpNotFound();

            db.GiangViens.Remove(gv);
            db.SaveChanges();
            TempData["ThongBao"] = "Xóa thành công!";
            return RedirectToAction("QuanLyGiangVien");
        }
        // GET: QuanLyGiangVien/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var giangVien = db.GiangViens.Find(id);
            if (giangVien == null)
                return HttpNotFound();

            return View(giangVien);
        }

        [HttpPost]
        public ActionResult Details(GiangVien giangVien)
        {
            // Kiểm tra tên giảng viên
            if (string.IsNullOrWhiteSpace(giangVien.TenGV))
            {
                ViewBag.ThongBao = "Vui lòng nhập tên giảng viên.";
                return View("Details", giangVien);
            }

            // Kiểm tra ngày sinh (nếu cần kiểm tra tuổi hay định dạng thêm tại đây)

            // Kiểm tra giới tính hợp lệ
            var dsGioiTinh = new List<string> { "Nam", "Nữ", "Khác" };
            if (string.IsNullOrWhiteSpace(giangVien.GioiTinh) || !dsGioiTinh.Contains(giangVien.GioiTinh))
            {
                ViewBag.ThongBao = "Giới tính không hợp lệ.";
                return View("Details", giangVien);
            }

            // Kiểm tra địa chỉ
            if (string.IsNullOrWhiteSpace(giangVien.DiaChi))
            {
                ViewBag.ThongBao = "Vui lòng nhập địa chỉ.";
                return View("Details", giangVien);
            }

            // Kiểm tra chuyên môn
            if (string.IsNullOrWhiteSpace(giangVien.ChuyenMon))
            {
                ViewBag.ThongBao = "Vui lòng nhập chuyên môn.";
                return View("Details", giangVien);
            }

            // Kiểm tra bằng cấp
            if (string.IsNullOrWhiteSpace(giangVien.BangCap))
            {
                ViewBag.ThongBao = "Vui lòng nhập bằng cấp.";
                return View("Details", giangVien);
            }

            // Nếu ModelState hợp lệ
            if (ModelState.IsValid)
            {
                var existing = db.GiangViens.FirstOrDefault(g => g.IDGiangVien == giangVien.IDGiangVien);
                if (existing != null)
                {
                    // Cập nhật thông tin
                    existing.TenGV = giangVien.TenGV;
                    existing.NgaySinh = giangVien.NgaySinh;
                    existing.GioiTinh = giangVien.GioiTinh;
                    existing.DiaChi = giangVien.DiaChi;
                    existing.ChuyenMon = giangVien.ChuyenMon;
                    existing.BangCap = giangVien.BangCap;

                    db.SaveChanges();
                    TempData["ThongBao"] = "Cập nhật giảng viên thành công!";
                    return RedirectToAction("QuanLyGiangVien");
                }
                else
                {
                    ViewBag.ThongBao = "Không tìm thấy giảng viên.";
                    return RedirectToAction("QuanLyGiangVien");

                }
            }

            ViewBag.ThongBao = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
            return View("QuanLyGiangVien", giangVien);
        }

    }
}
