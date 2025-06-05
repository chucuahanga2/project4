using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;
using PagedList;

namespace DuAnEnglish.Controllers
{
    public class XemThongBaoController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: DanhSachThongBao
        public ActionResult DanhSachThongBao(int? page)
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
        // GET: ChiTietThongBao
        public ActionResult ChiTietThongBao(int id)
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
                var giangVien = thongBao.TaiKhoan?.GiangViens?.FirstOrDefault();
                tenNguoiGui = giangVien != null ? giangVien.TenGV : "Không rõ";
            }

            ViewBag.TenNguoiGui = tenNguoiGui;

            return View(thongBao);
        }
    }
}