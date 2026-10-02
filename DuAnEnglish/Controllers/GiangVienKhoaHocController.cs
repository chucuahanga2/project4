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
    [AuthorizeRole("giangvien")]
    public class GiangVienKhoaHocController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        private GiangVien GetCurrentGiangVien()
        {
            if (Session["User"] == null) return null;
            string username = Session["User"].ToString();
            return db.GiangViens.FirstOrDefault(g => g.IDTenDangNhap == username);
        }

        // GET: GiangVienKhoaHoc
        public ActionResult Index()
        {
            var gv = GetCurrentGiangVien();
            if (gv == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hồ sơ giảng viên của bạn!";
                return RedirectToAction("Index", "HomeGiangVien");
            }

            var dsKhoaHoc = db.KhoaHocs
                              .Include(k => k.DanhMucKhoaHoc)
                              .Include(k => k.ChuongHocs)
                              .Include(k => k.DangKyKhoaHocs)
                              .Where(k => k.IDGiangVien == gv.IDGiangVien)
                              .OrderByDescending(k => k.NgayTao)
                              .ToList();

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(dsKhoaHoc);
        }

        // GET: GiangVienKhoaHoc/ThemKhoaHoc
        public ActionResult ThemKhoaHoc()
        {
            ViewBag.DanhSachDanhMuc = new SelectList(db.DanhMucKhoaHocs.Where(d => d.TrangThai == "Hoạt động"), "IDDanhMuc", "TenDanhMuc");
            return View();
        }

        // POST: GiangVienKhoaHoc/ThemKhoaHoc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemKhoaHoc(KhoaHoc kh, HttpPostedFileBase HinhAnhFile)
        {
            var gv = GetCurrentGiangVien();
            if (gv == null) return RedirectToAction("DangNhap", "DangNhap");

            ViewBag.DanhSachDanhMuc = new SelectList(db.DanhMucKhoaHocs.Where(d => d.TrangThai == "Hoạt động"), "IDDanhMuc", "TenDanhMuc", kh.IDDanhMuc);

            if (string.IsNullOrWhiteSpace(kh.IDKhoaHoc) || string.IsNullOrWhiteSpace(kh.TenKhoaHoc))
            {
                ViewBag.ThongBao = "Vui lòng nhập mã và tên khóa học!";
                return View(kh);
            }

            kh.IDKhoaHoc = kh.IDKhoaHoc.Trim();
            if (db.KhoaHocs.Any(k => k.IDKhoaHoc.ToLower() == kh.IDKhoaHoc.ToLower()))
            {
                ViewBag.ThongBao = "Mã khóa học này đã tồn tại, vui lòng chọn mã khác!";
                return View(kh);
            }

            // Xử lý upload ảnh bìa
            if (HinhAnhFile != null && HinhAnhFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(HinhAnhFile.FileName);
                string fileName = "kh_" + Guid.NewGuid().ToString().Substring(0, 8) + ext;
                string dir = Server.MapPath("~/Images/");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                HinhAnhFile.SaveAs(Path.Combine(dir, fileName));
                kh.HinhAnhKH = fileName;
            }

            kh.IDGiangVien = gv.IDGiangVien;
            kh.NgayTao = DateTime.Now;
            kh.TrangThai = "Hiển thị";
            if (!kh.HocPhi.HasValue) kh.HocPhi = 0;

            if (kh.IDDanhMuc.HasValue)
            {
                var dm = db.DanhMucKhoaHocs.Find(kh.IDDanhMuc.Value);
                if (dm != null) kh.DanhMuc = dm.TenDanhMuc;
            }

            db.KhoaHocs.Add(kh);
            db.SaveChanges();

            TempData["ThongBao"] = "Thêm khóa học mới thành công!";
            return RedirectToAction("ChiTietNoiDung", new { id = kh.IDKhoaHoc });
        }

        // GET: GiangVienKhoaHoc/SuaKhoaHoc/MVC2026
        public ActionResult SuaKhoaHoc(string id)
        {
            var gv = GetCurrentGiangVien();
            var kh = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == id && k.IDGiangVien == gv.IDGiangVien);
            if (kh == null)
            {
                TempData["ThongBao"] = "Bạn không có quyền chỉnh sửa khóa học này!";
                return RedirectToAction("Index");
            }

            ViewBag.DanhSachDanhMuc = new SelectList(db.DanhMucKhoaHocs.Where(d => d.TrangThai == "Hoạt động"), "IDDanhMuc", "TenDanhMuc", kh.IDDanhMuc);
            return View(kh);
        }

        // POST: GiangVienKhoaHoc/SuaKhoaHoc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaKhoaHoc(KhoaHoc kh, HttpPostedFileBase HinhAnhFile)
        {
            var gv = GetCurrentGiangVien();
            var existing = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == kh.IDKhoaHoc && k.IDGiangVien == gv.IDGiangVien);
            if (existing == null)
            {
                TempData["ThongBao"] = "Bạn không có quyền chỉnh sửa khóa học này!";
                return RedirectToAction("Index");
            }

            ViewBag.DanhSachDanhMuc = new SelectList(db.DanhMucKhoaHocs.Where(d => d.TrangThai == "Hoạt động"), "IDDanhMuc", "TenDanhMuc", kh.IDDanhMuc);

            if (string.IsNullOrWhiteSpace(kh.TenKhoaHoc))
            {
                ViewBag.ThongBao = "Tên khóa học không được để trống!";
                return View(kh);
            }

            // Xử lý upload ảnh mới nếu có
            if (HinhAnhFile != null && HinhAnhFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(HinhAnhFile.FileName);
                string fileName = "kh_" + Guid.NewGuid().ToString().Substring(0, 8) + ext;
                string dir = Server.MapPath("~/Images/");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                HinhAnhFile.SaveAs(Path.Combine(dir, fileName));
                existing.HinhAnhKH = fileName;
            }

            existing.TenKhoaHoc = kh.TenKhoaHoc;
            existing.IDDanhMuc = kh.IDDanhMuc;
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
            return RedirectToAction("Index");
        }

        // GET: GiangVienKhoaHoc/ChiTietNoiDung/MVC2026 (Quản lý Chương và Bài)
        public ActionResult ChiTietNoiDung(string id)
        {
            var gv = GetCurrentGiangVien();
            var kh = db.KhoaHocs
                       .Include(k => k.ChuongHocs.Select(c => c.BaiHocs))
                       .FirstOrDefault(k => k.IDKhoaHoc == id && k.IDGiangVien == gv.IDGiangVien);
            if (kh == null)
            {
                TempData["ThongBao"] = "Bạn không có quyền quản lý nội dung khóa học này!";
                return RedirectToAction("Index");
            }

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(kh);
        }

        // POST: Thêm chương học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemChuong(string idKhoaHoc, string tenChuong, string moTa, int? thuTu)
        {
            var gv = GetCurrentGiangVien();
            var kh = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == idKhoaHoc && k.IDGiangVien == gv.IDGiangVien);
            if (kh == null) return HttpNotFound();

            if (!string.IsNullOrWhiteSpace(tenChuong))
            {
                var chuong = new ChuongHoc
                {
                    IDKhoaHoc = idKhoaHoc,
                    TenChuong = tenChuong.Trim(),
                    MoTa = moTa,
                    ThuTu = thuTu ?? (kh.ChuongHocs.Count + 1)
                };
                db.ChuongHocs.Add(chuong);
                db.SaveChanges();
                TempData["ThongBao"] = "Đã thêm chương học mới!";
            }

            return RedirectToAction("ChiTietNoiDung", new { id = idKhoaHoc });
        }

        // POST: Xóa chương học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaChuong(int idChuong)
        {
            var gv = GetCurrentGiangVien();
            var chuong = db.ChuongHocs.Include(c => c.KhoaHoc).FirstOrDefault(c => c.IDChuong == idChuong);
            if (chuong == null || chuong.KhoaHoc.IDGiangVien != gv.IDGiangVien)
            {
                TempData["ThongBao"] = "Không tìm thấy chương học hoặc bạn không có quyền!";
                return RedirectToAction("Index");
            }

            string idKhoaHoc = chuong.IDKhoaHoc;
            db.ChuongHocs.Remove(chuong);
            db.SaveChanges();
            TempData["ThongBao"] = "Đã xóa chương học thành công!";
            return RedirectToAction("ChiTietNoiDung", new { id = idKhoaHoc });
        }

        // POST: Thêm bài học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemBaiHoc(int idChuong, string tenBaiHoc, string videoUrl, string taiLieuUrl, string moTa, int? thoiLuong, bool? choXemThu, int? thuTu)
        {
            var gv = GetCurrentGiangVien();
            var chuong = db.ChuongHocs.Include(c => c.KhoaHoc).FirstOrDefault(c => c.IDChuong == idChuong);
            if (chuong == null || chuong.KhoaHoc.IDGiangVien != gv.IDGiangVien)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrWhiteSpace(tenBaiHoc))
            {
                // Chuẩn hóa link YouTube nếu người dùng paste link xem thông thường
                string formattedVideo = videoUrl;
                if (!string.IsNullOrEmpty(formattedVideo) && formattedVideo.Contains("youtube.com/watch?v="))
                {
                    formattedVideo = formattedVideo.Replace("watch?v=", "embed/");
                }
                else if (!string.IsNullOrEmpty(formattedVideo) && formattedVideo.Contains("youtu.be/"))
                {
                    formattedVideo = formattedVideo.Replace("youtu.be/", "www.youtube.com/embed/");
                }

                var bai = new BaiHoc
                {
                    IDChuong = idChuong,
                    TenBaiHoc = tenBaiHoc.Trim(),
                    VideoUrl = formattedVideo,
                    TaiLieuUrl = taiLieuUrl,
                    MoTa = moTa,
                    ThoiLuong = thoiLuong ?? 10,
                    ChoXemThu = choXemThu ?? false,
                    ThuTu = thuTu ?? (chuong.BaiHocs.Count + 1)
                };
                db.BaiHocs.Add(bai);
                db.SaveChanges();
                TempData["ThongBao"] = "Đã thêm bài học thành công!";
            }

            return RedirectToAction("ChiTietNoiDung", new { id = chuong.IDKhoaHoc });
        }

        // POST: Xóa bài học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaBaiHoc(int idBaiHoc)
        {
            var gv = GetCurrentGiangVien();
            var bai = db.BaiHocs.Include(b => b.ChuongHoc.KhoaHoc).FirstOrDefault(b => b.IDBaiHoc == idBaiHoc);
            if (bai == null || bai.ChuongHoc.KhoaHoc.IDGiangVien != gv.IDGiangVien)
            {
                TempData["ThongBao"] = "Không tìm thấy bài học hoặc bạn không có quyền!";
                return RedirectToAction("Index");
            }

            string idKhoaHoc = bai.ChuongHoc.IDKhoaHoc;
            db.BaiHocs.Remove(bai);
            db.SaveChanges();
            TempData["ThongBao"] = "Đã xóa bài học thành công!";
            return RedirectToAction("ChiTietNoiDung", new { id = idKhoaHoc });
        }

        // GET: Danh sách học viên đã đăng ký khóa học này
        public ActionResult DanhSachHocVien(string id)
        {
            var gv = GetCurrentGiangVien();
            var kh = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == id && k.IDGiangVien == gv.IDGiangVien);
            if (kh == null)
            {
                TempData["ThongBao"] = "Bạn không có quyền truy cập khóa học này!";
                return RedirectToAction("Index");
            }

            var dsDangKy = db.DangKyKhoaHocs
                             .Include(d => d.HocVien)
                             .Include(d => d.HocVien.TaiKhoan)
                             .Where(d => d.IDKhoaHoc == id)
                             .OrderByDescending(d => d.NgayDangKy)
                             .ToList();

            ViewBag.KhoaHoc = kh;
            return View(dsDangKy);
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
