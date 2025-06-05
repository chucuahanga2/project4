using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;


namespace DuAnEnglish.Controllers
{
    public class QuanLyTaiKhoanController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: QuanLyTaiKhoan
        public ActionResult QuanLyTaiKhoan(string loaitaikhoan, string search)
        {
            var ds = db.TaiKhoans.ToList();

            if (!string.IsNullOrEmpty(loaitaikhoan) && loaitaikhoan != "all")
            {
                ds = ds.Where(k => k.LoaiTK == loaitaikhoan).ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                string keyword = search.ToLower();
                ds = ds.Where(t =>
                    (!string.IsNullOrEmpty(t.TenDangNhap) && t.TenDangNhap.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(t.Email) && t.Email.ToLower().Contains(keyword))
                ).ToList();
            }

            // Bỏ tài khoản admin ra khỏi danh sách
            ds = ds.Where(t => t.TenDangNhap.ToLower() != "admin").ToList();

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(ds);
        }

        // GET: Create
        public ActionResult Create()
        {
            ViewBag.LoaiTK = new SelectList(db.LoaiTaiKhoans, "LoaiTK", "LoaiTK");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string TenDangNhap, string LoaiTK)
        {
            // Kiểm tra tên đăng nhập có dấu cách không
            if (TenDangNhap.Contains(" "))
            {
                ViewBag.ThongBao = "Tên đăng nhập không được chứa dấu cách.";
            }
            else
            {
                // Kiểm tra nếu tài khoản đã tồn tại
                var existingTaiKhoan = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == TenDangNhap && t.LoaiTK == LoaiTK);
                if (existingTaiKhoan != null)
                {
                    ViewBag.ThongBao = "Tài khoản đã tồn tại.";
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        // Tạo tài khoản mới
                        TaiKhoan tk = new TaiKhoan
                        {
                            TenDangNhap = TenDangNhap,
                            LoaiTK = LoaiTK,
                            MatKhau = "123",            // Mật khẩu mặc định
                            Email = null,               // Email để null
                            SDT = null,                 // SĐT để null
                            TrangThai = "Hoạt động"     // Trạng thái mặc định
                        };

                        db.TaiKhoans.Add(tk);
                        db.SaveChanges();

                        // Thêm vào bảng HocVien hoặc GiangVien tương ứng
                        if (LoaiTK.ToLower() == "hocvien")
                        {
                            HocVien hv = new HocVien
                            {
                                IDTenDangNhap = TenDangNhap,
                                TenHV = null,
                                NgaySinh = null,
                                GioiTinh = null,
                                DiaChi = null
                            };
                            db.HocViens.Add(hv);
                        }
                        else if (LoaiTK.ToLower() == "giangvien")
                        {
                            GiangVien gv = new GiangVien
                            {
                                IDTenDangNhap = TenDangNhap,
                                TenGV = null,
                                NgaySinh = null,
                                GioiTinh = null,
                                DiaChi = null
                            };
                            db.GiangViens.Add(gv);
                        }

                        db.SaveChanges();

                        TempData["ThongBao"] = "Thêm tài khoản thành công";
                        return RedirectToAction("QuanLyTaiKhoan");
                    }
                }
            }

            // Nếu có lỗi, load lại danh sách loại tài khoản
            ViewBag.LoaiTKList = db.LoaiTaiKhoans.Select(l => new SelectListItem
            {
                Value = l.LoaiTK,
                Text = l.LoaiTK
            }).ToList();

            return View();
        }




        // GET: Edit
        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            TaiKhoan tk = db.TaiKhoans.Find(id);
            if (tk == null)
                return HttpNotFound();

            ViewBag.LoaiTK = new SelectList(db.LoaiTaiKhoans, "LoaiTK", "LoaiTK", tk.LoaiTK);
            return View(tk);
        }

       

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaiKhoan tk)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tk).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QuanLyTaiKhoan");
            }

            ViewBag.LoaiTK = new SelectList(db.LoaiTaiKhoans, "LoaiTK", "LoaiTK", tk.LoaiTK);
            return View(tk);
        }

        // GET: Delete
        public ActionResult Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var tk = db.TaiKhoans.Find(id);
            if (tk == null)
                return HttpNotFound();

            // Gán null trong bảng ThanhToan
            var thanhToans = db.ThanhToans.Where(t => t.TenDangNhap == id).ToList();
            foreach (var item in thanhToans)
            {
                item.TenDangNhap = null;
            }

            // Gán null trong bảng ThongBao
            var thongBaos = db.ThongBaos.Where(t => t.IDNguoiGui == id).ToList();
            foreach (var item in thongBaos)
            {
                item.IDNguoiGui = null;
            }

            // Nếu là học viên
            var hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == id);
            if (hocVien != null)
            {
                
                // Xóa trong bảng DiemTOEIC
                var diemTOEICs = db.DiemTOEICs.Where(d => d.IDHocVien == hocVien.IDHocVien).ToList();
                db.DiemTOEICs.RemoveRange(diemTOEICs);

                // Xóa trong bảng DiemIELTS
                var diemIELTSs = db.DiemIELTS.Where(d => d.IDHocVien == hocVien.IDHocVien).ToList();
                db.DiemIELTS.RemoveRange(diemIELTSs);

                // Xóa trong HocVienLopHoc
                var hocVienLopHocs = db.HocVienLopHocs.Where(h => h.IDHocVien == hocVien.IDHocVien).ToList();
                db.HocVienLopHocs.RemoveRange(hocVienLopHocs);

                // Xóa bản ghi HocVien
                db.HocViens.Remove(hocVien);
            }

            // Nếu là giảng viên
            var giangVien = db.GiangViens.FirstOrDefault(g => g.IDTenDangNhap == id);
            if (giangVien != null)
            {
                // Gán null trong bảng LopHoc trước khi xóa giảng viên
                var lopHocs = db.LopHocs.Where(l => l.IDGiangVien == giangVien.IDGiangVien).ToList();
                foreach (var lop in lopHocs)
                {
                    lop.IDGiangVien = null;
                }

                db.GiangViens.Remove(giangVien);
            }

            // Xóa tài khoản
            db.TaiKhoans.Remove(tk);
            db.SaveChanges();

            TempData["ThongBao"] = "Xóa tài khoản thành công.";
            return RedirectToAction("QuanLyTaiKhoan");
        }



        // GET
        public ActionResult Details(string id)
        {
            var taiKhoan = db.TaiKhoans.Find(id);
            if (taiKhoan == null)
            {
                return HttpNotFound();
            }
            return View(taiKhoan);
        }
        // POST: QuanLyTaiKhoan/Details

        [HttpPost]
        public ActionResult Details(TaiKhoan taiKhoan)
        {
            if (string.IsNullOrWhiteSpace(taiKhoan.Email))
            {
                ViewBag.ThongBao = "Vui lòng nhập địa chỉ email.";
                return View("Details", taiKhoan);
            }

            // Kiểm tra số điện thoại có hợp lệ hay không
            if (!string.IsNullOrWhiteSpace(taiKhoan.SDT) && !taiKhoan.SDT.All(char.IsDigit))
            {
                ViewBag.ThongBao = "Số điện thoại không hợp lệ.";
                return View("Details", taiKhoan);
            }

            // Kiểm tra loại tài khoản
            if (string.IsNullOrWhiteSpace(taiKhoan.LoaiTK) || !new List<string> { "admin", "giangvien", "hocvien" }.Contains(taiKhoan.LoaiTK))
            {
                ViewBag.ThongBao = "Loại tài khoản không hợp lệ.";
                return View("Details", taiKhoan);
            }

            if (ModelState.IsValid)
            {
                var existing = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == taiKhoan.TenDangNhap);
                if (existing != null)
                {
                    existing.Email = taiKhoan.Email;
                    existing.SDT = taiKhoan.SDT;
                    existing.TrangThai = taiKhoan.TrangThai;
                    existing.LoaiTK = taiKhoan.LoaiTK;

                    db.SaveChanges();
                    TempData["ThongBao"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction("QuanLyTaiKhoan");
                }
                else
                {
                    ViewBag.ThongBao = "Không tìm thấy tài khoản.";
                    return View("Details", taiKhoan);
                }
            }

            ViewBag.ThongBao = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
            return View("Details", taiKhoan);
        }       
    }
}
