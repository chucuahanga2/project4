using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class QuanLyKhoaHocController : Controller
    {
        // Giả sử bạn có DbContext tên là 'EnglishDbContext'
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyKhoaHoc
        // Tham số danhMuc để lọc khóa học (all, TOEIC, IELTS)
        public ActionResult QuanLyKhoaHoc(string danhmuc = "all", string search = "")
        {
            List<KhoaHoc> danhSachKhoaHoc;

            if (string.Equals(danhmuc, "all", StringComparison.OrdinalIgnoreCase))
            {
                danhSachKhoaHoc = db.KhoaHocs.ToList();
            }
            else
            {
                danhSachKhoaHoc = db.KhoaHocs
                    .Where(kh => kh.DanhMuc != null && kh.DanhMuc.Equals(danhmuc, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Tìm kiếm theo IDKhoaHoc
            if (!string.IsNullOrEmpty(search))
            {
                danhSachKhoaHoc = danhSachKhoaHoc
                    .Where(kh => kh.IDKhoaHoc.ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(danhSachKhoaHoc);
        }

        // GET: Details khóa học
        public ActionResult Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            KhoaHoc khoaHoc = db.KhoaHocs.Find(id);
            if (khoaHoc == null)
                return HttpNotFound();

            return View(khoaHoc);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(KhoaHoc kh, HttpPostedFileBase ImageFile)
        {
            ViewBag.DanhMucList = new List<string> { "TOEIC", "IELTS" };

            // Kiểm tra dữ liệu trống
            if (string.IsNullOrWhiteSpace(kh.TenKhoaHoc) ||
                string.IsNullOrWhiteSpace(kh.DanhMuc) ||
                kh.HocPhi <= 0 ||
                string.IsNullOrWhiteSpace(kh.MoTa))
            {
                ViewBag.ThongBao = "Không được để trống thông tin.";
                return View(kh);
            }

            var existing = db.KhoaHocs.Find(kh.IDKhoaHoc);
            if (existing == null)
            {
                ViewBag.ThongBao = "Không tìm thấy khóa học cần cập nhật.";
                return View(kh);
            }

            // Cập nhật thông tin
            existing.TenKhoaHoc = kh.TenKhoaHoc;
            existing.DanhMuc = kh.DanhMuc;
            existing.HocPhi = kh.HocPhi;
            existing.MoTa = kh.MoTa;

            // Hình ảnh
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                if (!Directory.Exists(Server.MapPath("~/Images/")))
                    Directory.CreateDirectory(Server.MapPath("~/Images/"));
                ImageFile.SaveAs(path);
                existing.HinhAnhKH = fileName;
            }

            db.SaveChanges();
            TempData["ThongBao"] = "Cập nhật thành công!";
            return RedirectToAction("QuanLyKhoaHoc");
        }


        // GET: Create khóa học (hiển thị form)
        public ActionResult Create()
        {
            // Có thể truyền ViewBag nếu cần danh mục
            ViewBag.DanhMucList = new List<string> { "TOEIC", "IELTS" };
            return View();
        }

        // POST: Create khóa học (xử lý lưu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhoaHoc khoaHoc, HttpPostedFileBase ImageFile)
        {
            ViewBag.DanhMucList = new List<string> { "TOEIC", "IELTS" };

            // Kiểm tra trường bắt buộc
            if (string.IsNullOrWhiteSpace(khoaHoc.IDKhoaHoc) || string.IsNullOrWhiteSpace(khoaHoc.TenKhoaHoc) ||
                string.IsNullOrWhiteSpace(khoaHoc.DanhMuc) || khoaHoc.HocPhi == 0 || string.IsNullOrWhiteSpace(khoaHoc.MoTa))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin.";
                return View(khoaHoc);
            }

            // Kiểm tra trùng ID
            if (db.KhoaHocs.Any(k => k.IDKhoaHoc == khoaHoc.IDKhoaHoc))
            {
                ViewBag.ThongBao = "ID khóa học đã tồn tại.";
                return View(khoaHoc);
            }

            // Xử lý upload hình ảnh
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ImageFile.FileName);
                string path = Path.Combine(Server.MapPath("~/Images/"), fileName);

                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(Server.MapPath("~/Images/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Images/"));
                }

                ImageFile.SaveAs(path);
                khoaHoc.HinhAnhKH = fileName;
            }

            db.KhoaHocs.Add(khoaHoc);
            db.SaveChanges();
            TempData["ThongBao"] = "Thêm khóa học thành công!";
            return RedirectToAction("QuanLyKhoaHoc");
        }


        // Delete khóa học (xác nhận)
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var khoaHoc = db.KhoaHocs.Find(id);
            if (khoaHoc == null)
                return HttpNotFound();

            // Xử lý ràng buộc: Gán IDKhoaHoc = null cho các bản ghi có liên kết
            var lopHocs = db.LopHocs.Where(lh => lh.IDKhoaHoc == id).ToList();
            foreach (var lh in lopHocs)
            {
                lh.IDKhoaHoc = null;
            }

            var thanhToans = db.ThanhToans.Where(tt => tt.IDKhoaHoc == id).ToList();
            foreach (var tt in thanhToans)
            {
                tt.IDKhoaHoc = null;
            }

            // Xóa hình ảnh khỏi thư mục nếu có
            if (!string.IsNullOrEmpty(khoaHoc.HinhAnhKH))
            {
                string imagePath = Path.Combine(Server.MapPath("~/Images/"), khoaHoc.HinhAnhKH);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            db.KhoaHocs.Remove(khoaHoc);
            db.SaveChanges();

            TempData["ThongBao"] = "Xóa khóa học thành công!";
            return RedirectToAction("QuanLyKhoaHoc");
        }

    }
}
