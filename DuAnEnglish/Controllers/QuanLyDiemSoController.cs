using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.ViewModels;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("giangvien")]
    public class QuanLyDiemSoController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyDiemSo
        public ActionResult QuanLyDiemSo(string idKhoaHoc, string tuKhoa)
        {
            string tenDangNhap = Session["User"] != null ? Session["User"].ToString() : null;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để quản lý điểm số!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var giangVien = db.GiangViens.FirstOrDefault(gv => gv.IDTenDangNhap == tenDangNhap);
            if (giangVien == null)
            {
                TempData["ThongBao"] = "Không tìm thấy thông tin giảng viên!";
                return RedirectToAction("Index", "Home");
            }

            // Danh sách các khóa học do giảng viên này phụ trách
            var danhSachKhoaHoc = db.KhoaHocs
                                    .Where(kh => kh.IDGiangVien == giangVien.IDGiangVien)
                                    .OrderBy(kh => kh.TenKhoaHoc)
                                    .ToList();

            ViewBag.DanhSachKhoaHoc = danhSachKhoaHoc;
            ViewBag.SelectedKhoaHoc = idKhoaHoc;
            ViewBag.TuKhoa = tuKhoa;

            var khoaHocIds = danhSachKhoaHoc.Select(k => k.IDKhoaHoc).ToList();

            // Truy vấn các lượt đăng ký khóa học online
            var query = db.DangKyKhoaHocs
                          .Include(dk => dk.HocVien)
                          .Include(dk => dk.KhoaHoc)
                          .Include(dk => dk.DiemKhoaHocs)
                          .Where(dk => khoaHocIds.Contains(dk.IDKhoaHoc));

            if (!string.IsNullOrEmpty(idKhoaHoc))
            {
                query = query.Where(dk => dk.IDKhoaHoc == idKhoaHoc);
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                string kw = tuKhoa.Trim().ToLower();
                query = query.Where(dk => (dk.HocVien.TenHV != null && dk.HocVien.TenHV.ToLower().Contains(kw))
                                       || (dk.HocVien.IDTenDangNhap != null && dk.HocVien.IDTenDangNhap.ToLower().Contains(kw))
                                       || (dk.KhoaHoc.TenKhoaHoc != null && dk.KhoaHoc.TenKhoaHoc.ToLower().Contains(kw)));
            }

            var danhSachDangKy = query.OrderByDescending(dk => dk.NgayDangKy).ToList();
            var danhSachDiem = new List<DiemViewModel>();

            foreach (var dk in danhSachDangKy)
            {
                int tongSoBai = db.BaiHocs.Count(b => b.ChuongHoc.IDKhoaHoc == dk.IDKhoaHoc);
                int soBaiDaHoc = db.TienDoHocs.Count(t => t.IDHocVien == dk.IDHocVien && t.DaHoanThanh == true && t.BaiHoc.ChuongHoc.IDKhoaHoc == dk.IDKhoaHoc);
                int phanTram = tongSoBai > 0 ? (int)Math.Round((double)soBaiDaHoc / tongSoBai * 100) : 0;

                var diemItem = dk.DiemKhoaHocs.FirstOrDefault();

                danhSachDiem.Add(new DiemViewModel
                {
                    IDDangKy = dk.IDDangKy,
                    IDHocVien = dk.IDHocVien,
                    TenHocVien = dk.HocVien != null ? dk.HocVien.TenHV : "Học viên #" + dk.IDHocVien,
                    TenDangNhap = dk.HocVien != null ? dk.HocVien.IDTenDangNhap : "",
                    IDKhoaHoc = dk.IDKhoaHoc,
                    TenKhoaHoc = dk.KhoaHoc != null ? dk.KhoaHoc.TenKhoaHoc : dk.IDKhoaHoc,
                    TongSoBaiHoc = tongSoBai,
                    SoBaiDaHoc = soBaiDaHoc,
                    TienDoPhanTram = phanTram,
                    Diem = diemItem != null ? diemItem.Diem : null,
                    NhanXet = diemItem != null ? diemItem.NhanXet : null,
                    NgayCapNhat = diemItem != null ? diemItem.NgayCapNhat : null,
                    TrangThaiDangKy = dk.TrangThai
                });
            }

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(danhSachDiem);
        }

        // GET: QuanLyDiemSo/NhapDiem/5
        public ActionResult NhapDiem(int idDangKy)
        {
            string tenDangNhap = Session["User"] != null ? Session["User"].ToString() : null;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var giangVien = db.GiangViens.FirstOrDefault(gv => gv.IDTenDangNhap == tenDangNhap);
            if (giangVien == null)
            {
                TempData["ThongBao"] = "Không tìm thấy thông tin giảng viên!";
                return RedirectToAction("Index", "Home");
            }

            var dangKy = db.DangKyKhoaHocs
                           .Include(d => d.HocVien)
                           .Include(d => d.KhoaHoc)
                           .Include(d => d.DiemKhoaHocs)
                           .FirstOrDefault(d => d.IDDangKy == idDangKy);

            if (dangKy == null)
            {
                return HttpNotFound();
            }

            // Bảo mật: Giảng viên chỉ được nhập điểm cho khóa học do chính mình phụ trách
            if (dangKy.KhoaHoc == null || dangKy.KhoaHoc.IDGiangVien != giangVien.IDGiangVien)
            {
                TempData["ThongBao"] = "Bạn không có quyền quản lý điểm của khóa học này!";
                return RedirectToAction("QuanLyDiemSo");
            }

            int tongSoBai = db.BaiHocs.Count(b => b.ChuongHoc.IDKhoaHoc == dangKy.IDKhoaHoc);
            int soBaiDaHoc = db.TienDoHocs.Count(t => t.IDHocVien == dangKy.IDHocVien && t.DaHoanThanh == true && t.BaiHoc.ChuongHoc.IDKhoaHoc == dangKy.IDKhoaHoc);
            int phanTram = tongSoBai > 0 ? (int)Math.Round((double)soBaiDaHoc / tongSoBai * 100) : 0;

            var diemItem = dangKy.DiemKhoaHocs.FirstOrDefault();

            var viewModel = new NhapDiemViewModel
            {
                IDDangKy = dangKy.IDDangKy,
                IDHocVien = dangKy.IDHocVien,
                TenHocVien = dangKy.HocVien != null ? dangKy.HocVien.TenHV : "Học viên #" + dangKy.IDHocVien,
                TenDangNhap = dangKy.HocVien != null ? dangKy.HocVien.IDTenDangNhap : "",
                IDKhoaHoc = dangKy.IDKhoaHoc,
                TenKhoaHoc = dangKy.KhoaHoc != null ? dangKy.KhoaHoc.TenKhoaHoc : dangKy.IDKhoaHoc,
                TongSoBaiHoc = tongSoBai,
                SoBaiDaHoc = soBaiDaHoc,
                TienDoPhanTram = phanTram,
                Diem = diemItem != null ? diemItem.Diem : null,
                NhanXet = diemItem != null ? diemItem.NhanXet : null,
                NgayCapNhat = diemItem != null ? diemItem.NgayCapNhat : null
            };

            return View(viewModel);
        }

        // POST: QuanLyDiemSo/NhapDiem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NhapDiem(int idDangKy, string diem, string nhanXet)
        {
            string tenDangNhap = Session["User"] != null ? Session["User"].ToString() : null;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var giangVien = db.GiangViens.FirstOrDefault(gv => gv.IDTenDangNhap == tenDangNhap);
            if (giangVien == null)
            {
                TempData["ThongBao"] = "Không tìm thấy thông tin giảng viên!";
                return RedirectToAction("Index", "Home");
            }

            var dangKy = db.DangKyKhoaHocs
                           .Include(d => d.KhoaHoc)
                           .Include(d => d.HocVien)
                           .Include(d => d.DiemKhoaHocs)
                           .FirstOrDefault(d => d.IDDangKy == idDangKy);

            if (dangKy == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra phân quyền sở hữu khóa học
            if (dangKy.KhoaHoc == null || dangKy.KhoaHoc.IDGiangVien != giangVien.IDGiangVien)
            {
                TempData["ThongBao"] = "Bạn không có quyền quản lý điểm của khóa học này!";
                return RedirectToAction("QuanLyDiemSo");
            }

            decimal? parsedDiem = null;
            bool isValidScore = true;

            if (!string.IsNullOrWhiteSpace(diem))
            {
                string normalized = diem.Trim().Replace(',', '.');
                decimal val;
                if (decimal.TryParse(normalized, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out val))
                {
                    if (val < 0m || val > 10m)
                    {
                        isValidScore = false;
                    }
                    else
                    {
                        parsedDiem = Math.Round(val, 2);
                    }
                }
                else
                {
                    isValidScore = false;
                }
            }

            // Kiểm tra hợp lệ điểm số phía Server: 0 <= Diem <= 10
            if (!isValidScore)
            {
                ViewBag.ThongBao = "Điểm số không hợp lệ! Vui lòng nhập điểm từ 0.0 đến 10.0.";

                int tongSoBai = db.BaiHocs.Count(b => b.ChuongHoc.IDKhoaHoc == dangKy.IDKhoaHoc);
                int soBaiDaHoc = db.TienDoHocs.Count(t => t.IDHocVien == dangKy.IDHocVien && t.DaHoanThanh == true && t.BaiHoc.ChuongHoc.IDKhoaHoc == dangKy.IDKhoaHoc);
                int phanTram = tongSoBai > 0 ? (int)Math.Round((double)soBaiDaHoc / tongSoBai * 100) : 0;

                var vm = new NhapDiemViewModel
                {
                    IDDangKy = dangKy.IDDangKy,
                    IDHocVien = dangKy.IDHocVien,
                    TenHocVien = dangKy.HocVien != null ? dangKy.HocVien.TenHV : "",
                    TenDangNhap = dangKy.HocVien != null ? dangKy.HocVien.IDTenDangNhap : "",
                    IDKhoaHoc = dangKy.IDKhoaHoc,
                    TenKhoaHoc = dangKy.KhoaHoc != null ? dangKy.KhoaHoc.TenKhoaHoc : "",
                    TongSoBaiHoc = tongSoBai,
                    SoBaiDaHoc = soBaiDaHoc,
                    TienDoPhanTram = phanTram,
                    Diem = parsedDiem,
                    NhanXet = nhanXet
                };
                return View(vm);
            }

            var diemItem = db.DiemKhoaHocs.FirstOrDefault(d => d.IDDangKy == idDangKy);
            if (diemItem == null)
            {
                diemItem = new DiemKhoaHoc
                {
                    IDDangKy = idDangKy,
                    Diem = parsedDiem,
                    NhanXet = nhanXet,
                    NgayCapNhat = DateTime.Now
                };
                db.DiemKhoaHocs.Add(diemItem);
            }
            else
            {
                diemItem.Diem = parsedDiem;
                diemItem.NhanXet = nhanXet;
                diemItem.NgayCapNhat = DateTime.Now;
            }

            db.SaveChanges();
            TempData["ThongBao"] = "Lưu điểm khóa học thành công!";
            return RedirectToAction("QuanLyDiemSo");
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
