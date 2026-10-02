using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;
using PagedList;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("giangvien")]
    public class QuanLyThongBaoGVController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: QuanLyThongBaoGV
        //public ActionResult QuanLyThongBaoGV()
        //{
        //    string tenDangNhap = Session["User"] as string;

        //    if (string.IsNullOrEmpty(tenDangNhap))
        //    {
        //        TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
        //        return RedirectToAction("DangNhap", "DangNhap");
        //    }

        //    var danhSachThongBao = db.ThongBaos
        //        .OrderByDescending(tb => tb.NgayGui)
        //        .ToList();
        //    if (TempData["ThongBao"] != null)
        //    {
        //        ViewBag.ThongBao = TempData["ThongBao"];
        //    }
        //    return View(danhSachThongBao);
        //}
        // GET: QuanLyThongBaoGV/Index
        public ActionResult Index(int? page)
        {
            return QuanLyThongBaoGV(page);
        }

        public ActionResult QuanLyThongBaoGV(int? page)
        {
            string tenDangNhap = Session["User"] as string;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            int pageSize = 5; // số thông báo trên mỗi trang
            int pageNumber = (page ?? 1); // nếu null thì mặc định trang 1

            var danhSachThongBao = db.ThongBaos
                .OrderByDescending(tb => tb.NgayGui)
                .ToPagedList(pageNumber, pageSize);

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(danhSachThongBao);
        }
        public ActionResult Them()
        {
            return View();
        }
        // POST: QuanLyThongBaoGV/Them
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them(string TieuDe, string NoiDung)
        {
            string tenDangNhap = Session["User"] as string;

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            if (!string.IsNullOrEmpty(TieuDe) && !string.IsNullOrEmpty(NoiDung))
            {
                // Tạo đối tượng thông báo mới
                var thongBaoMoi = new ThongBao
                {
                    TieuDe = TieuDe,
                    NoiDung = NoiDung,
                    NgayGui = DateTime.Now,
                    IDNguoiGui = tenDangNhap // Giả sử IDNguoiGui là tên đăng nhập của người gửi
                };

                // Thêm thông báo vào cơ sở dữ liệu
                db.ThongBaos.Add(thongBaoMoi);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm thông báo thành công";
                // Chuyển hướng về danh sách thông báo
                return RedirectToAction("QuanLyThongBaoGV");
            }

            // Nếu Tiêu Đề hoặc Nội Dung rỗng, hiển thị lại form
            ViewBag.ThongBao = "Bạn chưa nhập nội dung cho tiêu đề hoặc nội dung.";
            return View();
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

            var thongBao = db.ThongBaos.FirstOrDefault(tb => tb.IDThongBao == id && tb.IDNguoiGui == tenDangNhap);

            if (thongBao == null)
            {
                TempData["ThongBao"] = "Thông báo không tồn tại hoặc bạn không có quyền xóa.";
                return RedirectToAction("QuanLyThongBaoGV");
            }

            db.ThongBaos.Remove(thongBao);
            db.SaveChanges();

            TempData["ThongBao"] = "Xóa thông báo thành công";
            return RedirectToAction("QuanLyThongBaoGV");
        }

        public ActionResult Xem(int id)
        {
            var thongBao = db.ThongBaos
                    .Where(tb => tb.IDThongBao == id)
                    .FirstOrDefault();

            if (thongBao == null)
            {
                return HttpNotFound();
            }

            string tenNguoiGui;

            if (thongBao.IDNguoiGui.ToLower() == "admin")
            {
                tenNguoiGui = "Giám Đốc Trung Tâm";
            }
            else
            {
                var giangVien = (thongBao.TaiKhoan != null && thongBao.TaiKhoan.GiangViens != null) ? thongBao.TaiKhoan.GiangViens.FirstOrDefault() : null;
                tenNguoiGui = giangVien != null ? giangVien.TenGV : "Không rõ";
            }

            ViewBag.TenNguoiGui = tenNguoiGui;

            return View(thongBao);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(ThongBao thongBao)
        {
            string tenDangNhap = Session["User"] as string;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var thongBaoCu = db.ThongBaos.FirstOrDefault(tb => tb.IDThongBao == thongBao.IDThongBao && tb.IDNguoiGui == tenDangNhap);

            if (thongBaoCu == null)
            {
                ViewBag.ThongBao = "Thông báo không tồn tại hoặc bạn không có quyền chỉnh sửa.";
                return View("Xem", thongBao);
            }

            thongBaoCu.TieuDe = thongBao.TieuDe;
            thongBaoCu.NoiDung = thongBao.NoiDung;
            db.SaveChanges();

            ViewBag.ThongBao = "Sửa thành công"; // Thông báo thành công
            string tenNguoiGui;

            if (thongBaoCu.IDNguoiGui.ToLower() == "admin")
            {
                tenNguoiGui = "Giám Đốc Trung Tâm";
            }
            else
            {
                var tkNguoiGui = db.TaiKhoans.Find(thongBaoCu.IDNguoiGui);
                var giangVien = (tkNguoiGui != null && tkNguoiGui.GiangViens != null) ? tkNguoiGui.GiangViens.FirstOrDefault() : null;
                tenNguoiGui = giangVien != null ? giangVien.TenGV : "Không rõ";
            }

            ViewBag.TenNguoiGui = tenNguoiGui;

            return View("Xem", thongBaoCu); // Quay lại trang chi tiết thông báo
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Sua(ThongBao thongBao)
        //{
        //    var thongBaoCu = db.ThongBaos.FirstOrDefault(tb => tb.IDThongBao == thongBao.IDThongBao);
        //    if (thongBaoCu == null)
        //    {
        //        TempData["ThongBao"] = "Thông báo không tồn tại";
        //        return RedirectToAction("QuanLyThongBaoGV");
        //    }

        //    thongBaoCu.TieuDe = thongBao.TieuDe;
        //    thongBaoCu.NoiDung = thongBao.NoiDung;
        //    db.SaveChanges();

        //    TempData["ThongBao"] = "Sửa thành công";
        //    return RedirectToAction("QuanLyThongBaoGV");
        //}
    }
}