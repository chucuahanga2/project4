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
    public class QuanLyHocVienController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyHocVien/Index
        public ActionResult Index(string search = "")
        {
            return RedirectToAction("QuanLyHocVien", new { search });
        }

        // GET: QuanLyHocVien
        public ActionResult QuanLyHocVien(string search = "")
        {
            var query = db.HocViens.Include(h => h.TaiKhoan).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(h => h.IDHocVien.ToString().Contains(kw) ||
                                        (h.TenHV != null && h.TenHV.ToLower().Contains(kw)) ||
                                        (h.IDTenDangNhap != null && h.IDTenDangNhap.ToLower().Contains(kw)) ||
                                        (h.TaiKhoan != null && h.TaiKhoan.Email != null && h.TaiKhoan.Email.ToLower().Contains(kw)));
            }

            var dsHocVien = query.OrderByDescending(h => h.IDHocVien).ToList();

            ViewBag.CurrentSearch = search;
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(dsHocVien);
        }

        // GET: QuanLyHocVien/ChiTiet/5
        public ActionResult ChiTiet(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var hocVien = db.HocViens
                            .Include(h => h.TaiKhoan)
                            .Include(h => h.DangKyKhoaHocs.Select(d => d.KhoaHoc))
                            .Include(h => h.DangKyKhoaHocs.Select(d => d.DiemKhoaHocs))
                            .FirstOrDefault(h => h.IDHocVien == id);

            if (hocVien == null) return HttpNotFound();

            // Tính toán tiến độ học tập cho từng khóa học đã đăng ký
            var tienDoDict = new System.Collections.Generic.Dictionary<int, Tuple<int, int, int>>();
            if (hocVien.DangKyKhoaHocs != null)
            {
                foreach (var dk in hocVien.DangKyKhoaHocs)
                {
                    int tongSoBai = db.BaiHocs.Count(b => b.ChuongHoc.IDKhoaHoc == dk.IDKhoaHoc);
                    int soBaiDaHoc = db.TienDoHocs.Count(t => t.IDHocVien == hocVien.IDHocVien && t.DaHoanThanh == true && t.BaiHoc.ChuongHoc.IDKhoaHoc == dk.IDKhoaHoc);
                    int phanTram = tongSoBai > 0 ? (int)Math.Round((double)soBaiDaHoc * 100 / tongSoBai) : 0;
                    if (phanTram > 100) phanTram = 100;

                    tienDoDict[dk.IDDangKy] = Tuple.Create(soBaiDaHoc, tongSoBai, phanTram);
                }
            }
            ViewBag.TienDoDict = tienDoDict;

            return View(hocVien);
        }

        // POST: Khóa hoặc Mở khóa tài khoản học viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KhoaMoTaiKhoan(int id)
        {
            var hocVien = db.HocViens.Include(h => h.TaiKhoan).FirstOrDefault(h => h.IDHocVien == id);
            if (hocVien == null || hocVien.TaiKhoan == null)
            {
                TempData["ThongBao"] = "Không tìm thấy tài khoản học viên!";
                return RedirectToAction("QuanLyHocVien");
            }

            if (hocVien.TaiKhoan.TrangThai == "Khóa")
            {
                hocVien.TaiKhoan.TrangThai = "Hoạt động";
                TempData["ThongBao"] = string.Format("Đã mở khóa thành công tài khoản học viên '{0}'!", hocVien.TenHV ?? hocVien.IDTenDangNhap);
            }
            else
            {
                hocVien.TaiKhoan.TrangThai = "Khóa";
                TempData["ThongBao"] = string.Format("Đã khóa tài khoản học viên '{0}'!", hocVien.TenHV ?? hocVien.IDTenDangNhap);
            }

            db.SaveChanges();
            return RedirectToAction("QuanLyHocVien");
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