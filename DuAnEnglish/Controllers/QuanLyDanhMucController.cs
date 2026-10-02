using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("admin")]
    public class QuanLyDanhMucController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyDanhMuc
        public ActionResult Index(string search = "")
        {
            var query = db.DanhMucKhoaHocs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(d => d.TenDanhMuc.ToLower().Contains(kw) || 
                                        (d.MoTa != null && d.MoTa.ToLower().Contains(kw)));
            }

            var dsDanhMuc = query.OrderBy(d => d.ThuTu).ThenBy(d => d.IDDanhMuc).ToList();

            ViewBag.CurrentSearch = search;
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(dsDanhMuc);
        }

        // GET: QuanLyDanhMuc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuanLyDanhMuc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DanhMucKhoaHoc dm)
        {
            if (string.IsNullOrWhiteSpace(dm.TenDanhMuc))
            {
                ViewBag.ThongBao = "Vui lòng nhập tên danh mục!";
                return View(dm);
            }

            dm.TenDanhMuc = dm.TenDanhMuc.Trim();
            if (db.DanhMucKhoaHocs.Any(d => d.TenDanhMuc.ToLower() == dm.TenDanhMuc.ToLower()))
            {
                ViewBag.ThongBao = "Tên danh mục này đã tồn tại!";
                return View(dm);
            }

            if (string.IsNullOrEmpty(dm.TrangThai))
            {
                dm.TrangThai = "Hoạt động";
            }

            if (!dm.ThuTu.HasValue)
            {
                dm.ThuTu = 0;
            }

            db.DanhMucKhoaHocs.Add(dm);
            db.SaveChanges();

            TempData["ThongBao"] = "Thêm danh mục thành công!";
            return RedirectToAction("Index");
        }

        // GET: QuanLyDanhMuc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var dm = db.DanhMucKhoaHocs.Find(id);
            if (dm == null) return HttpNotFound();

            return View(dm);
        }

        // POST: QuanLyDanhMuc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DanhMucKhoaHoc dm)
        {
            if (string.IsNullOrWhiteSpace(dm.TenDanhMuc))
            {
                ViewBag.ThongBao = "Tên danh mục không được để trống!";
                return View(dm);
            }

            var existing = db.DanhMucKhoaHocs.Find(dm.IDDanhMuc);
            if (existing == null) return HttpNotFound();

            dm.TenDanhMuc = dm.TenDanhMuc.Trim();
            if (db.DanhMucKhoaHocs.Any(d => d.TenDanhMuc.ToLower() == dm.TenDanhMuc.ToLower() && d.IDDanhMuc != dm.IDDanhMuc))
            {
                ViewBag.ThongBao = "Tên danh mục đã bị trùng với danh mục khác!";
                return View(dm);
            }

            existing.TenDanhMuc = dm.TenDanhMuc;
            existing.MoTa = dm.MoTa;
            existing.TrangThai = dm.TrangThai;
            existing.ThuTu = dm.ThuTu;

            db.SaveChanges();
            TempData["ThongBao"] = "Cập nhật danh mục thành công!";
            return RedirectToAction("Index");
        }

        // POST: QuanLyDanhMuc/Delete/5
        public ActionResult Delete(int id)
        {
            var dm = db.DanhMucKhoaHocs.Find(id);
            if (dm == null)
            {
                TempData["ThongBao"] = "Không tìm thấy danh mục cần xóa.";
                return RedirectToAction("Index");
            }

            // Kiểm tra có khóa học nào đang thuộc danh mục này không
            int countKhoaHoc = db.KhoaHocs.Count(k => k.IDDanhMuc == id);
            if (countKhoaHoc > 0)
            {
                // Không xóa vật lý để tránh lỗi khóa ngoại, chuyển sang Khóa
                dm.TrangThai = "Khóa";
                db.SaveChanges();
                TempData["ThongBao"] = string.Format("Danh mục đang có {0} khóa học. Đã chuyển trạng thái sang 'Khóa' để đảm bảo an toàn dữ liệu.", countKhoaHoc);
                return RedirectToAction("Index");
            }

            db.DanhMucKhoaHocs.Remove(dm);
            db.SaveChanges();
            TempData["ThongBao"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
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
