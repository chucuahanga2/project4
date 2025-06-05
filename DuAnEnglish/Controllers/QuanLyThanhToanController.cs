using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class QuanLyThanhToanController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyThanhToan
        public ActionResult QuanLyThanhToan(string search)
        {
            var ds = db.ThanhToans.AsQueryable();

            // Tìm kiếm theo mã học viên (IDThanhToan) hoặc tên đăng nhập
            if (!string.IsNullOrEmpty(search))
            {
                ds = ds.Where(t =>
                    t.IDThanhToan.ToString().Contains(search) ||
                    t.TenDangNhap.Contains(search));
            }
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            return View(ds.ToList());
        }

        // GET: QuanLyThanhToan/ChiTiet/5
        public ActionResult ChiTiet(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var thanhToan = db.ThanhToans.Find(id);
            if (thanhToan == null) return HttpNotFound();

            return View(thanhToan);
        }


      // Xóa
        public ActionResult Delete(int? id)
        {
            var item = db.ThanhToans.Find(id);
            if (item != null)
            {
                db.ThanhToans.Remove(item);
                db.SaveChanges();
                TempData["ThongBao"] = "Xóa thành công!";
            }

            return RedirectToAction("QuanLyThanhToan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChiTiet(ThanhToan model)
        {
            var item = db.ThanhToans.Find(model.IDThanhToan);
            if (item == null) return HttpNotFound();

            item.TrangThai = model.TrangThai;
            item.NgayXacNhan = DateTime.Now;

            // Nếu trạng thái là "Đã thanh toán", tiến hành thêm vào 3 bảng
            if (model.TrangThai == "Đã thanh toán")
            {
                // Lấy học viên theo TenDangNhap
                var hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == item.TenDangNhap);
                if (hocVien != null && !string.IsNullOrEmpty(item.IDLopHoc))
                {
                    int idHocVien = hocVien.IDHocVien;
                    string idLopHoc = item.IDLopHoc;

                    // Kiểm tra nếu chưa tồn tại trong bảng HocVienLopHoc thì mới thêm
                    var existed = db.HocVienLopHocs.Any(hvl => hvl.IDHocVien == idHocVien && hvl.IDLopHoc == idLopHoc);
                    if (!existed)
                    {
                        // Thêm vào bảng HocVienLopHoc
                        db.HocVienLopHocs.Add(new HocVienLopHoc
                        {
                            IDHocVien = idHocVien,
                            IDLopHoc = idLopHoc
                        });

                        // Thêm vào bảng DiemIELTS
                        db.DiemIELTS.Add(new DiemIELT
                        {
                            IDHocVien = idHocVien,
                            IDLopHoc = idLopHoc,
                            DiemNghe = 0,
                            DiemNoi = 0,
                            DiemDoc = 0,
                            DiemViet = 0,
                            TongDiem = 0
                        });

                        // Thêm vào bảng DiemTOEIC
                        db.DiemTOEICs.Add(new DiemTOEIC
                        {
                            IDHocVien = idHocVien,
                            IDLopHoc = idLopHoc,
                            Part1 = 0,
                            Part2 = 0,
                            Part3 = 0,
                            Part4 = 0,
                            DiemNghe = 0,
                            Part5 = 0,
                            Part6 = 0,
                            Part7 = 0,
                            DiemDoc = 0,
                            DiemNoi = 0,
                            DiemViet = 0,
                            TongDiem = 0
                        });
                    }
                }
            }

            db.SaveChanges();
            TempData["ThongBao"] = "Cập nhật thành công!";
            return RedirectToAction("QuanLyThanhToan");
        }


    }
}
