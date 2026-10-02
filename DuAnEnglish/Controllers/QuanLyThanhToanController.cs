using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("admin")]
    public class QuanLyThanhToanController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyThanhToan
        public ActionResult QuanLyThanhToan(string search = "", string trangthai = "all")
        {
            var query = db.ThanhToans
                          .Include(t => t.KhoaHoc)
                          .Include(t => t.DangKyKhoaHoc)
                          .AsQueryable();

            if (!string.IsNullOrEmpty(trangthai) && trangthai != "all")
            {
                query = query.Where(t => t.TrangThai == trangthai);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(t => t.IDThanhToan.ToString().Contains(kw) ||
                                        (t.TenDangNhap != null && t.TenDangNhap.ToLower().Contains(kw)) ||
                                        (t.IDKhoaHoc != null && t.IDKhoaHoc.ToLower().Contains(kw)) ||
                                        (t.KhoaHoc != null && t.KhoaHoc.TenKhoaHoc.ToLower().Contains(kw)));
            }

            var dsThanhToan = query.OrderByDescending(t => t.IDThanhToan).ToList();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentTrangThai = trangthai;
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(dsThanhToan);
        }

        // GET: ChiTiet/5
        public ActionResult ChiTiet(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var thanhToan = db.ThanhToans
                              .Include(t => t.KhoaHoc)
                              .Include(t => t.GiaoDichVNPAYs)
                              .Include(t => t.DangKyKhoaHoc)
                              .FirstOrDefault(t => t.IDThanhToan == id);

            if (thanhToan == null) return HttpNotFound();

            return View(thanhToan);
        }

        // POST: ChiTiet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChiTiet(int IDThanhToan, string TrangThai)
        {
            var tt = db.ThanhToans.Include(t => t.DangKyKhoaHoc).FirstOrDefault(t => t.IDThanhToan == IDThanhToan);
            if (tt == null) return HttpNotFound();

            tt.TrangThai = TrangThai;
            if (TrangThai == "Đã thanh toán")
            {
                tt.NgayXacNhan = DateTime.Now;
                if (!tt.NgayThanhToan.HasValue) tt.NgayThanhToan = DateTime.Now;

                // Kích hoạt khóa học
                if (tt.IDDangKy.HasValue)
                {
                    var dk = db.DangKyKhoaHocs.Find(tt.IDDangKy.Value);
                    if (dk != null) dk.TrangThai = "Đã kích hoạt";
                }
                else
                {
                    var hv = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == tt.TenDangNhap);
                    if (hv != null)
                    {
                        var dk = db.DangKyKhoaHocs.FirstOrDefault(d => d.IDHocVien == hv.IDHocVien && d.IDKhoaHoc == tt.IDKhoaHoc);
                        if (dk != null) dk.TrangThai = "Đã kích hoạt";
                    }
                }
            }

            db.SaveChanges();
            TempData["ThongBao"] = string.Format("Cập nhật trạng thái hóa đơn #{0} thành '{1}' thành công!", IDThanhToan, TrangThai);
            return RedirectToAction("QuanLyThanhToan");
        }

        // POST: DuyetThanhToan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DuyetThanhToan(int id)
        {
            var tt = db.ThanhToans.Include(t => t.DangKyKhoaHoc).FirstOrDefault(t => t.IDThanhToan == id);
            if (tt == null) return HttpNotFound();

            tt.TrangThai = "Đã thanh toán";
            tt.NgayXacNhan = DateTime.Now;

            // Kích hoạt khóa học
            if (tt.IDDangKy.HasValue)
            {
                var dk = db.DangKyKhoaHocs.Find(tt.IDDangKy.Value);
                if (dk != null) dk.TrangThai = "Đã kích hoạt";
            }
            else
            {
                var hv = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == tt.TenDangNhap);
                if (hv != null)
                {
                    var dk = db.DangKyKhoaHocs.FirstOrDefault(d => d.IDHocVien == hv.IDHocVien && d.IDKhoaHoc == tt.IDKhoaHoc);
                    if (dk != null) dk.TrangThai = "Đã kích hoạt";
                }
            }

            db.SaveChanges();
            TempData["ThongBao"] = string.Format("Đã duyệt thanh toán thành công hóa đơn #{0} và kích hoạt quyền học cho học viên!", id);
            return RedirectToAction("QuanLyThanhToan");
        }

        // POST: Xóa hóa đơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var item = db.ThanhToans.Find(id);
            if (item != null)
            {
                var logs = db.GiaoDichVNPAYs.Where(g => g.IDThanhToan == id).ToList();
                db.GiaoDichVNPAYs.RemoveRange(logs);

                db.ThanhToans.Remove(item);
                db.SaveChanges();
                TempData["ThongBao"] = "Xóa hóa đơn thành công!";
            }

            return RedirectToAction("QuanLyThanhToan");
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
