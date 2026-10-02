using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("admin")]
    public class QuanLyKhoaHocController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyKhoaHoc
        public ActionResult QuanLyKhoaHoc(string danhmuc = "all", string search = "")
        {
            var query = db.KhoaHocs
                          .Include(k => k.DanhMucKhoaHoc)
                          .Include(k => k.GiangVien)
                          .Include(k => k.DangKyKhoaHocs)
                          .Include(k => k.ChuongHocs.Select(c => c.BaiHocs))
                          .AsQueryable();

            if (!string.IsNullOrEmpty(danhmuc) && danhmuc != "all")
            {
                int idDm;
                if (int.TryParse(danhmuc, out idDm))
                {
                    query = query.Where(k => k.IDDanhMuc == idDm);
                }
                else
                {
                    query = query.Where(k => k.DanhMuc == danhmuc);
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(k => k.IDKhoaHoc.ToLower().Contains(kw) || 
                                        k.TenKhoaHoc.ToLower().Contains(kw) || 
                                        (k.GiangVien != null && k.GiangVien.TenGV.ToLower().Contains(kw)));
            }

            var dsKhoaHoc = query.OrderByDescending(k => k.NgayTao).ToList();

            ViewBag.DanhSachDanhMuc = db.DanhMucKhoaHocs.Where(d => d.TrangThai == "Hoạt động").ToList();
            ViewBag.CurrentDanhMuc = danhmuc;
            ViewBag.CurrentSearch = search;

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(dsKhoaHoc);
        }

        private bool IsValidImageFile(HttpPostedFileBase file, out string errorMessage)
        {
            errorMessage = null;
            if (file == null || file.ContentLength == 0) return true;

            if (file.ContentLength > 5 * 1024 * 1024)
            {
                errorMessage = "Dung lượng ảnh bìa không được vượt quá 5MB!";
                return false;
            }

            string ext = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExtensions.Contains(ext))
            {
                errorMessage = "Chỉ chấp nhận các tệp ảnh hợp lệ (.jpg, .jpeg, .png, .webp)!";
                return false;
            }

            var allowedMimes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedMimes.Contains(file.ContentType.ToLower()))
            {
                errorMessage = "Định dạng MIME của tệp tin không hợp lệ!";
                return false;
            }

            return true;
        }

        // GET: Create
        public ActionResult Create()
        {
            ViewBag.IDDanhMuc = new SelectList(db.DanhMucKhoaHocs.Where(d => d.TrangThai == "Hoạt động"), "IDDanhMuc", "TenDanhMuc");
            ViewBag.IDGiangVien = new SelectList(db.GiangViens, "IDGiangVien", "TenGV");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhoaHoc khoaHoc, HttpPostedFileBase ImageFile)
        {
            ViewBag.IDDanhMuc = new SelectList(db.DanhMucKhoaHocs.Where(d => d.TrangThai == "Hoạt động"), "IDDanhMuc", "TenDanhMuc", khoaHoc.IDDanhMuc);
            ViewBag.IDGiangVien = new SelectList(db.GiangViens, "IDGiangVien", "TenGV", khoaHoc.IDGiangVien);

            if (string.IsNullOrWhiteSpace(khoaHoc.IDKhoaHoc) || string.IsNullOrWhiteSpace(khoaHoc.TenKhoaHoc))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ mã và tên khóa học!";
                return View(khoaHoc);
            }

            khoaHoc.IDKhoaHoc = khoaHoc.IDKhoaHoc.Trim();
            if (db.KhoaHocs.Any(k => k.IDKhoaHoc.ToLower() == khoaHoc.IDKhoaHoc.ToLower()))
            {
                ViewBag.ThongBao = "Mã khóa học đã tồn tại!";
                return View(khoaHoc);
            }

            // Kiểm tra tính hợp lệ của tệp ảnh
            string imgError;
            if (!IsValidImageFile(ImageFile, out imgError))
            {
                ViewBag.ThongBao = imgError;
                return View(khoaHoc);
            }

            // Xử lý upload ảnh
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(ImageFile.FileName).ToLower();
                string fileName = "kh_" + Guid.NewGuid().ToString().Substring(0, 8) + ext;
                string dir = Server.MapPath("~/Images/");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                ImageFile.SaveAs(Path.Combine(dir, fileName));
                khoaHoc.HinhAnhKH = fileName;
            }

            khoaHoc.NgayTao = DateTime.Now;
            if (string.IsNullOrEmpty(khoaHoc.TrangThai)) khoaHoc.TrangThai = "Hiển thị";
            if (!khoaHoc.HocPhi.HasValue) khoaHoc.HocPhi = 0;

            if (khoaHoc.IDDanhMuc.HasValue)
            {
                var dm = db.DanhMucKhoaHocs.Find(khoaHoc.IDDanhMuc.Value);
                if (dm != null) khoaHoc.DanhMuc = dm.TenDanhMuc;
            }

            db.KhoaHocs.Add(khoaHoc);
            db.SaveChanges();

            TempData["ThongBao"] = "Thêm mới khóa học thành công!";
            return RedirectToAction("QuanLyKhoaHoc");
        }

        // GET: Details / Edit
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var khoaHoc = db.KhoaHocs.Include(k => k.DanhMucKhoaHoc).Include(k => k.GiangVien).FirstOrDefault(k => k.IDKhoaHoc == id);
            if (khoaHoc == null) return HttpNotFound();

            ViewBag.IDDanhMuc = new SelectList(db.DanhMucKhoaHocs.Where(d => d.TrangThai == "Hoạt động"), "IDDanhMuc", "TenDanhMuc", khoaHoc.IDDanhMuc);
            ViewBag.IDGiangVien = new SelectList(db.GiangViens, "IDGiangVien", "TenGV", khoaHoc.IDGiangVien);
            return View(khoaHoc);
        }

        // POST: Details / Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(KhoaHoc kh, HttpPostedFileBase ImageFile)
        {
            ViewBag.IDDanhMuc = new SelectList(db.DanhMucKhoaHocs.Where(d => d.TrangThai == "Hoạt động"), "IDDanhMuc", "TenDanhMuc", kh.IDDanhMuc);
            ViewBag.IDGiangVien = new SelectList(db.GiangViens, "IDGiangVien", "TenGV", kh.IDGiangVien);

            if (string.IsNullOrWhiteSpace(kh.TenKhoaHoc))
            {
                ViewBag.ThongBao = "Tên khóa học không được để trống!";
                return View(kh);
            }

            var existing = db.KhoaHocs.Find(kh.IDKhoaHoc);
            if (existing == null) return HttpNotFound();

            // Kiểm tra tính hợp lệ của tệp ảnh
            string imgError;
            if (!IsValidImageFile(ImageFile, out imgError))
            {
                ViewBag.ThongBao = imgError;
                return View(kh);
            }

            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(ImageFile.FileName).ToLower();
                string fileName = "kh_" + Guid.NewGuid().ToString().Substring(0, 8) + ext;
                string dir = Server.MapPath("~/Images/");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                ImageFile.SaveAs(Path.Combine(dir, fileName));
                existing.HinhAnhKH = fileName;
            }

            existing.TenKhoaHoc = kh.TenKhoaHoc;
            existing.IDDanhMuc = kh.IDDanhMuc;
            existing.IDGiangVien = kh.IDGiangVien;
            existing.HocPhi = kh.HocPhi ?? 0;
            existing.MoTa = kh.MoTa;
            existing.NoiDung = kh.NoiDung;
            existing.TrangThai = kh.TrangThai;

            if (kh.IDDanhMuc.HasValue)
            {
                var dm = db.DanhMucKhoaHocs.Find(kh.IDDanhMuc.Value);
                if (dm != null) existing.DanhMuc = dm.TenDanhMuc;
            }

            db.SaveChanges();
            TempData["ThongBao"] = "Cập nhật khóa học thành công!";
            return RedirectToAction("QuanLyKhoaHoc");
        }

        // POST: Delete / Chuyển trạng thái Ẩn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var kh = db.KhoaHocs.Find(id);
            if (kh == null) return HttpNotFound();

            // Kiểm tra có học viên đã đăng ký hoặc thanh toán chưa
            int countDangKy = db.DangKyKhoaHocs.Count(d => d.IDKhoaHoc == id);
            int countThanhToan = db.ThanhToans.Count(t => t.IDKhoaHoc == id);

            if (countDangKy > 0 || countThanhToan > 0)
            {
                // Soft delete: chuyển sang trạng thái Ẩn
                kh.TrangThai = "Ẩn";
                db.SaveChanges();
                TempData["ThongBao"] = "Khóa học đã có học viên đăng ký hoặc phát sinh thanh toán. Đã tự động chuyển trạng thái sang 'Ẩn' để bảo đảm an toàn dữ liệu!";
                return RedirectToAction("QuanLyKhoaHoc");
            }

            db.KhoaHocs.Remove(kh);
            db.SaveChanges();
            TempData["ThongBao"] = "Xóa khóa học thành công!";
            return RedirectToAction("QuanLyKhoaHoc");
        }

        // GET: QuanLyKhoaHoc/NoiDungKhoaHoc/MVC2026 (Quản trị & duyệt nội dung bài học)
        public ActionResult NoiDungKhoaHoc(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var kh = db.KhoaHocs
                       .Include(k => k.GiangVien)
                       .Include(k => k.DanhMucKhoaHoc)
                       .Include(k => k.ChuongHocs.Select(c => c.BaiHocs))
                       .FirstOrDefault(k => k.IDKhoaHoc == id);

            if (kh == null) return HttpNotFound();

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(kh);
        }

        // POST: Duyệt cho phép hoặc khóa học thử của bài học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleXemThu(int idBaiHoc)
        {
            var bai = db.BaiHocs.Include(b => b.ChuongHoc).FirstOrDefault(b => b.IDBaiHoc == idBaiHoc);
            if (bai == null) return HttpNotFound();

            bai.ChoXemThu = !(bai.ChoXemThu == true);
            db.SaveChanges();
            TempData["ThongBao"] = string.Format("Đã cập nhật trạng thái học thử cho bài '{0}' thành '{1}'!", bai.TenBaiHoc, bai.ChoXemThu == true ? "Cho phép học thử" : "Khóa học thử");
            return RedirectToAction("NoiDungKhoaHoc", new { id = bai.ChuongHoc.IDKhoaHoc });
        }

        // POST: Xóa bài học vi phạm hoặc trùng lặp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaBaiHoc(int idBaiHoc)
        {
            var bai = db.BaiHocs.Include(b => b.ChuongHoc).FirstOrDefault(b => b.IDBaiHoc == idBaiHoc);
            if (bai == null) return HttpNotFound();

            string idKhoaHoc = bai.ChuongHoc.IDKhoaHoc;
            var tienDos = db.TienDoHocs.Where(t => t.IDBaiHoc == idBaiHoc).ToList();
            if (tienDos.Any())
            {
                db.TienDoHocs.RemoveRange(tienDos);
            }
            db.BaiHocs.Remove(bai);
            db.SaveChanges();
            TempData["ThongBao"] = "Đã xóa bài học thành công khỏi khóa học!";
            return RedirectToAction("NoiDungKhoaHoc", new { id = idKhoaHoc });
        }

        // POST: Xóa chương học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaChuongHoc(int idChuong)
        {
            var chuong = db.ChuongHocs.Include(c => c.BaiHocs).FirstOrDefault(c => c.IDChuong == idChuong);
            if (chuong == null) return HttpNotFound();

            string idKhoaHoc = chuong.IDKhoaHoc;
            if (chuong.BaiHocs != null && chuong.BaiHocs.Any())
            {
                foreach (var b in chuong.BaiHocs.ToList())
                {
                    var tienDos = db.TienDoHocs.Where(t => t.IDBaiHoc == b.IDBaiHoc).ToList();
                    if (tienDos.Any()) db.TienDoHocs.RemoveRange(tienDos);
                    db.BaiHocs.Remove(b);
                }
            }
            db.ChuongHocs.Remove(chuong);
            db.SaveChanges();
            TempData["ThongBao"] = "Đã xóa chương học và toàn bộ bài học thuộc chương!";
            return RedirectToAction("NoiDungKhoaHoc", new { id = idKhoaHoc });
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
