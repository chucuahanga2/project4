using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("admin")]
    public class QuanLyLopHocController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: QuanLyLopHoc
        public ActionResult QuanLyLopHoc()
        {
            string tenDangNhap = Session["User"] as string;

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            var dsLopHoc = db.LopHocs.ToList();
            return View(dsLopHoc);
        }
        //GET: hiển thị form lên
        public ActionResult Them()
        {
            ViewBag.IDPhongHoc = new SelectList(db.PhongHocs, "IDPhongHoc", "IDPhongHoc");
            ViewBag.IDKhoaHoc = new SelectList(db.KhoaHocs, "IDKhoaHoc", "IDKhoaHoc");
            ViewBag.IDGiangVien = new SelectList(db.GiangViens, "IDGiangVien", "TenGV");
            return View();
        }

        // POST: Xử lý thêm lớp học
        [HttpPost]
        public ActionResult Them(LopHoc lop)
        {
            ViewBag.IDPhongHoc = new SelectList(db.PhongHocs, "IDPhongHoc", "IDPhongHoc", lop.IDPhongHoc);
            ViewBag.IDKhoaHoc = new SelectList(db.KhoaHocs, "IDKhoaHoc", "IDKhoaHoc", lop.IDKhoaHoc);
            ViewBag.IDGiangVien = new SelectList(db.GiangViens, "IDGiangVien", "TenGV", lop.IDGiangVien);
            // Kiểm tra bắt buộc nhập đầy đủ thông tin
            if (string.IsNullOrWhiteSpace(lop.IDLopHoc) ||
            string.IsNullOrWhiteSpace(lop.TenLop) ||
            !lop.IDPhongHoc.HasValue ||
            string.IsNullOrWhiteSpace(lop.IDKhoaHoc) ||
            !lop.IDGiangVien.HasValue ||
            !lop.GioHocBD.HasValue ||
            !lop.GioHocKT.HasValue ||
            string.IsNullOrWhiteSpace(lop.ThuTrongTuan))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin!";
                return View(lop);
            }


            // Kiểm tra mã lớp đã tồn tại chưa
            var tonTaiMaLop = db.LopHocs.Any(l => l.IDLopHoc == lop.IDLopHoc);
            if (tonTaiMaLop)
            {
                ViewBag.ThongBao = "lớp học đã tồn tại, vui lòng nhập lại mã lớp học";
                return View(lop);
            }

            // Kiểm tra Slot
            if (lop.Slot < 0)
            {
                ViewBag.ThongBao = "Slot không được nhỏ hơn 0.";
                return View(lop);
            }

            var phong = db.PhongHocs.FirstOrDefault(p => p.IDPhongHoc == lop.IDPhongHoc);
            if (phong != null && lop.Slot > phong.SucChua)
            {
                ViewBag.ThongBao = string.Format("Phòng chỉ tối đa {0} học viên vui lòng điều chỉnh lại.", phong.SucChua);
                return View(lop);
            }

            try
            {
                db.LopHocs.Add(lop);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm lớp học thành công!";
                return RedirectToAction("QuanLyLopHoc");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = "Có lỗi xảy ra khi thêm lớp học: " + ex.Message;
                return View(lop);
            }
        }
        //GET
        public ActionResult Sua(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var lop = db.LopHocs.Find(id);
            if (lop == null)
            {
                return HttpNotFound();
            }

            ViewBag.IDPhongHoc = new SelectList(db.PhongHocs, "IDPhongHoc", "IDPhongHoc", lop.IDPhongHoc);
            ViewBag.IDKhoaHoc = new SelectList(db.KhoaHocs, "IDKhoaHoc", "IDKhoaHoc", lop.IDKhoaHoc);
            ViewBag.IDGiangVien = new SelectList(db.GiangViens, "IDGiangVien", "TenGV", lop.IDGiangVien);

            return View(lop);
        }
        [HttpPost]
        public ActionResult Sua(LopHoc lop)
        {
            // Đổ lại dữ liệu cho dropdown
            ViewBag.IDPhongHoc = new SelectList(db.PhongHocs, "IDPhongHoc", "IDPhongHoc", lop.IDPhongHoc);
            ViewBag.IDKhoaHoc = new SelectList(db.KhoaHocs, "IDKhoaHoc", "IDKhoaHoc", lop.IDKhoaHoc);
            ViewBag.IDGiangVien = new SelectList(db.GiangViens, "IDGiangVien", "TenGV", lop.IDGiangVien);

            // Kiểm tra ràng buộc bắt buộc
            if (
            string.IsNullOrWhiteSpace(lop.TenLop) ||
            !lop.IDPhongHoc.HasValue ||
            string.IsNullOrWhiteSpace(lop.IDKhoaHoc) ||
            !lop.IDGiangVien.HasValue ||
            !lop.GioHocBD.HasValue ||
            !lop.GioHocKT.HasValue ||
            string.IsNullOrWhiteSpace(lop.ThuTrongTuan))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin!";
                return View(lop);
            }
            
            if (lop.Slot < 0)
            {
                ViewBag.ThongBao = "Slot không được nhỏ hơn 0.";
                return View(lop);
            }

            // Kiểm tra sức chứa phòng học
            var phong = db.PhongHocs.FirstOrDefault(p => p.IDPhongHoc == lop.IDPhongHoc);
            if (phong != null && lop.Slot > phong.SucChua)
            {
                ViewBag.ThongBao = string.Format("Phòng chỉ tối đa {0} học viên. Vui lòng điều chỉnh lại.", phong.SucChua);
                return View(lop);
            }

            // Tìm lớp cũ để cập nhật
            var lopCu = db.LopHocs.Find(lop.IDLopHoc);
            if (lopCu == null)
            {
                ViewBag.ThongBao = "Không tìm thấy lớp học cần sửa.";
                return View(lop);
            }

            // Cập nhật thông tin lớp học
            lopCu.TenLop = lop.TenLop;
            lopCu.IDPhongHoc = lop.IDPhongHoc;
            lopCu.Slot = lop.Slot;
            lopCu.IDKhoaHoc = lop.IDKhoaHoc;
            lopCu.IDGiangVien = lop.IDGiangVien;
            lopCu.GioHocBD = lop.GioHocBD;
            lopCu.GioHocKT = lop.GioHocKT;
            lopCu.ThuTrongTuan = lop.ThuTrongTuan;

            try
            {
                db.SaveChanges();
                TempData["ThongBao"] = "Sửa thông tin lớp học thành công!";
                return RedirectToAction("QuanLyLopHoc");
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = "Có lỗi xảy ra khi cập nhật: " + ex.Message;
                return View(lop);
            }
        }

        public ActionResult Xoa(string id)
        {
            var lop = db.LopHocs.Find(id);
            if (lop == null)
            {
                TempData["ThongBao"] = "Không tìm thấy lớp học cần xóa!";
                return RedirectToAction("QuanLyLopHoc");
            }

            try
            {
                // Đặt IDLopHoc = null trong bảng ThanhToan
                var thanhToans = db.ThanhToans.Where(t => t.IDLopHoc == id).ToList();
                foreach (var t in thanhToans)
                {
                    t.IDLopHoc = null;
                }
                // Xóa điểm IELTS liên quan
                var ielts = db.DiemIELTS.Where(d => d.IDLopHoc == id).ToList();
                db.DiemIELTS.RemoveRange(ielts);

                // Xóa điểm TOEIC liên quan
                var toeic = db.DiemTOEICs.Where(d => d.IDLopHoc == id).ToList();
                db.DiemTOEICs.RemoveRange(toeic);

                // Xóa học viên lớp học liên quan
                var hvLop = db.HocVienLopHocs.Where(hv => hv.IDLopHoc == id).ToList();
                db.HocVienLopHocs.RemoveRange(hvLop);

                // Xóa lớp học
                db.LopHocs.Remove(lop);

                db.SaveChanges();
                TempData["ThongBao"] = "Xóa lớp học thành công!";
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Lỗi khi xóa lớp học: " + ex.Message;
            }

            return RedirectToAction("QuanLyLopHoc");
        }

    }
}