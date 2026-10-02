using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("admin")]
    public class QuanLyTaiKhoanController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyTaiKhoan/Index
        public ActionResult Index(string loaitaikhoan = "all", string search = "")
        {
            return RedirectToAction("QuanLyTaiKhoan", new { loaitaikhoan, search });
        }

        // GET: QuanLyTaiKhoan
        public ActionResult QuanLyTaiKhoan(string loaitaikhoan = "all", string search = "")
        {
            var query = db.TaiKhoans.AsQueryable();

            if (!string.IsNullOrEmpty(loaitaikhoan) && loaitaikhoan != "all")
            {
                query = query.Where(t => t.LoaiTK == loaitaikhoan);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(t => t.TenDangNhap.ToLower().Contains(kw) ||
                                        (t.Email != null && t.Email.ToLower().Contains(kw)) ||
                                        (t.SDT != null && t.SDT.Contains(kw)));
            }

            var dsTaiKhoan = query.OrderBy(t => t.TenDangNhap).ToList();

            ViewBag.CurrentLoaiTK = loaitaikhoan;
            ViewBag.CurrentSearch = search;
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(dsTaiKhoan);
        }

        // GET: Create
        public ActionResult Create()
        {
            ViewBag.LoaiTKList = new SelectList(new[] { "admin", "giangvien", "hocvien" });
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string TenDangNhap, string MatKhau, string LoaiTK, string Email, string SDT, string HoTen)
        {
            ViewBag.LoaiTKList = new SelectList(new[] { "admin", "giangvien", "hocvien" }, LoaiTK);

            if (string.IsNullOrWhiteSpace(TenDangNhap) || string.IsNullOrWhiteSpace(MatKhau) || string.IsNullOrWhiteSpace(LoaiTK))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ tên đăng nhập, mật khẩu và loại tài khoản!";
                return View();
            }

            TenDangNhap = TenDangNhap.Trim();
            if (db.TaiKhoans.Any(t => t.TenDangNhap.ToLower() == TenDangNhap.ToLower()))
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            var tk = new TaiKhoan
            {
                TenDangNhap = TenDangNhap,
                MatKhau = PasswordHelper.HashPassword(MatKhau),
                LoaiTK = LoaiTK,
                Email = Email,
                SDT = SDT,
                TrangThai = "Hoạt động"
            };
            db.TaiKhoans.Add(tk);

            // Bổ sung bản ghi thực thể tương ứng
            if (LoaiTK == "hocvien")
            {
                var hv = new HocVien
                {
                    IDTenDangNhap = TenDangNhap,
                    TenHV = string.IsNullOrEmpty(HoTen) ? TenDangNhap : HoTen
                };
                db.HocViens.Add(hv);
            }
            else if (LoaiTK == "giangvien")
            {
                var gv = new GiangVien
                {
                    IDTenDangNhap = TenDangNhap,
                    TenGV = string.IsNullOrEmpty(HoTen) ? TenDangNhap : HoTen
                };
                db.GiangViens.Add(gv);
            }

            db.SaveChanges();
            TempData["ThongBao"] = "Thêm tài khoản mới thành công!";
            return RedirectToAction("QuanLyTaiKhoan");
        }

        // GET: Details / Edit
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var tk = db.TaiKhoans.Find(id);
            if (tk == null) return HttpNotFound();

            ViewBag.LoaiTKList = new SelectList(new[] { "admin", "giangvien", "hocvien" }, tk.LoaiTK);
            return View(tk);
        }

        // POST: Details / Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(TaiKhoan model)
        {
            var existing = db.TaiKhoans.Find(model.TenDangNhap);
            if (existing == null) return HttpNotFound();

            existing.Email = model.Email;
            existing.SDT = model.SDT;
            existing.LoaiTK = model.LoaiTK;
            existing.TrangThai = model.TrangThai;

            db.SaveChanges();
            TempData["ThongBao"] = "Cập nhật thông tin tài khoản thành công!";
            return RedirectToAction("QuanLyTaiKhoan");
        }

        // POST: Khóa / Mở khóa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KhoaMoTaiKhoan(string id)
        {
            var tk = db.TaiKhoans.Find(id);
            if (tk == null) return HttpNotFound();

            if (tk.TenDangNhap.ToLower() == "admin")
            {
                TempData["ThongBao"] = "Không thể khóa tài khoản quản trị tối cao (admin)!";
                return RedirectToAction("QuanLyTaiKhoan");
            }

            if (tk.TrangThai == "Khóa")
            {
                tk.TrangThai = "Hoạt động";
                TempData["ThongBao"] = string.Format("Đã mở khóa tài khoản '{0}' thành công!", tk.TenDangNhap);
            }
            else
            {
                tk.TrangThai = "Khóa";
                TempData["ThongBao"] = string.Format("Đã khóa tài khoản '{0}'!", tk.TenDangNhap);
            }

            db.SaveChanges();
            return RedirectToAction("QuanLyTaiKhoan");
        }

        // POST: Đặt lại mật khẩu về mặc định (được băm an toàn)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetMatKhau(string id)
        {
            var tk = db.TaiKhoans.Find(id);
            if (tk == null) return HttpNotFound();

            tk.MatKhau = PasswordHelper.HashPassword("123456");
            db.SaveChanges();

            TempData["ThongBao"] = string.Format("Đã đặt lại mật khẩu tài khoản '{0}' về '123456' an toàn!", tk.TenDangNhap);
            return RedirectToAction("QuanLyTaiKhoan");
        }

        // POST: Xóa tài khoản an toàn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var tk = db.TaiKhoans.Find(id);
            if (tk == null) return HttpNotFound();

            if (tk.TenDangNhap.ToLower() == "admin")
            {
                TempData["ThongBao"] = "Không thể xóa tài khoản admin gốc!";
                return RedirectToAction("QuanLyTaiKhoan");
            }

            // Kiểm tra có thanh toán hoặc đăng ký không
            bool coThanhToan = db.ThanhToans.Any(t => t.TenDangNhap == id);
            if (coThanhToan)
            {
                tk.TrangThai = "Khóa";
                db.SaveChanges();
                TempData["ThongBao"] = "Tài khoản đã có lịch sử giao dịch thanh toán. Đã chuyển trạng thái sang 'Khóa' để bảo lưu lịch sử kế toán!";
                return RedirectToAction("QuanLyTaiKhoan");
            }

            // Xóa hồ sơ tương ứng
            var hv = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == id);
            if (hv != null) db.HocViens.Remove(hv);

            var gv = db.GiangViens.FirstOrDefault(g => g.IDTenDangNhap == id);
            if (gv != null) db.GiangViens.Remove(gv);

            db.TaiKhoans.Remove(tk);
            db.SaveChanges();

            TempData["ThongBao"] = "Xóa tài khoản thành công!";
            return RedirectToAction("QuanLyTaiKhoan");
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
