using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.ViewModels;

namespace DuAnEnglish.Controllers
{
    public class HocTapController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: HocTap/Index
        public ActionResult Index()
        {
            return RedirectToAction("KhoaHocCuaToi");
        }

        // GET: HocTap/KhoaHocCuaToi
        public ActionResult KhoaHocCuaToi()
        {
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Vui lòng đăng nhập để xem các khóa học của bạn!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string tenDangNhap = Session["User"].ToString();
            var hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == tenDangNhap);
            if (hocVien == null)
            {
                TempData["ThongBaoDangNhap"] = "Không tìm thấy hồ sơ học viên!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            // Lấy danh sách các khóa học đã đăng ký thành công (kích hoạt)
            var dsDangKy = db.DangKyKhoaHocs
                             .Include(d => d.KhoaHoc)
                             .Include(d => d.KhoaHoc.GiangVien)
                             .Include(d => d.DiemKhoaHocs)
                             .Where(d => d.IDHocVien == hocVien.IDHocVien && 
                                        (d.TrangThai == "Đã kích hoạt" || d.TrangThai == "Đã hoàn thành"))
                             .OrderByDescending(d => d.NgayDangKy)
                             .ToList();

            var ketQua = new List<KhoaHocCuaToiItem>();

            foreach (var dk in dsDangKy)
            {
                var kh = dk.KhoaHoc;
                if (kh == null) continue;

                // Đếm tổng số bài học của khóa học
                int tongSoBai = db.BaiHocs.Count(b => b.ChuongHoc.IDKhoaHoc == kh.IDKhoaHoc);

                // Đếm số bài học đã hoàn thành
                int soBaiHoanThanh = db.TienDoHocs.Count(t => t.IDHocVien == hocVien.IDHocVien &&
                                                             t.DaHoanThanh == true &&
                                                             t.BaiHoc.ChuongHoc.IDKhoaHoc == kh.IDKhoaHoc);

                // Tìm bài học gần nhất đã xem hoặc bài học đầu tiên
                var tienDoGanNhat = db.TienDoHocs
                                      .Where(t => t.IDHocVien == hocVien.IDHocVien && t.BaiHoc.ChuongHoc.IDKhoaHoc == kh.IDKhoaHoc)
                                      .OrderByDescending(t => t.ThoiDiemXemGanNhat)
                                      .FirstOrDefault();

                int? nextBaiId = null;
                string nextBaiName = null;

                if (tienDoGanNhat != null && tienDoGanNhat.BaiHoc != null)
                {
                    nextBaiId = tienDoGanNhat.IDBaiHoc;
                    nextBaiName = tienDoGanNhat.BaiHoc.TenBaiHoc;
                }
                else
                {
                    // Lấy bài học đầu tiên
                    var baiDauTien = db.BaiHocs
                                       .Where(b => b.ChuongHoc.IDKhoaHoc == kh.IDKhoaHoc)
                                       .OrderBy(b => b.ChuongHoc.ThuTu)
                                       .ThenBy(b => b.ThuTu)
                                       .FirstOrDefault();
                    if (baiDauTien != null)
                    {
                        nextBaiId = baiDauTien.IDBaiHoc;
                        nextBaiName = baiDauTien.TenBaiHoc;
                    }
                }

                var diemItem = dk.DiemKhoaHocs != null ? dk.DiemKhoaHocs.FirstOrDefault() : null;

                ketQua.Add(new KhoaHocCuaToiItem
                {
                    IDKhoaHoc = kh.IDKhoaHoc,
                    TenKhoaHoc = kh.TenKhoaHoc,
                    HinhAnhKH = kh.HinhAnhKH,
                    TenGiangVien = kh.GiangVien != null ? kh.GiangVien.TenGV : "Giảng viên hệ thống",
                    NgayDangKy = dk.NgayDangKy,
                    TongSoBai = tongSoBai,
                    SoBaiDaHoanThanh = soBaiHoanThanh,
                    IDBaiHocTiepTheo = nextBaiId,
                    TenBaiHocTiepTheo = nextBaiName,
                    Diem = diemItem != null ? diemItem.Diem : null,
                    NhanXet = diemItem != null ? diemItem.NhanXet : null,
                    NgayCapNhatDiem = diemItem != null ? diemItem.NgayCapNhat : null
                });
            }

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(ketQua);
        }

        // GET: HocTap/VaoHoc/MVC2026?baiId=1
        public ActionResult VaoHoc(string id, int? baiId, string idKhoaHoc = null)
        {
            if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(idKhoaHoc))
            {
                id = idKhoaHoc;
            }

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("KhoaHocCuaToi");
            }

            var khoaHoc = db.KhoaHocs
                            .Include(k => k.GiangVien)
                            .Include(k => k.ChuongHocs.Select(c => c.BaiHocs))
                            .FirstOrDefault(k => k.IDKhoaHoc == id);

            if (khoaHoc == null)
            {
                return HttpNotFound("Không tìm thấy khóa học.");
            }

            // 1. Kiểm tra quyền truy cập khóa học & hỗ trợ xem thử (ChoXemThu)
            bool coQuyenHoc = false;
            bool isHocThu = false;
            HocVien hocVien = null;

            if (Session["User"] != null)
            {
                string tenDangNhap = Session["User"].ToString();
                string role = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";

                if (role == "admin")
                {
                    coQuyenHoc = true;
                }
                else if (role == "giangvien")
                {
                    var gv = db.GiangViens.FirstOrDefault(g => g.IDTenDangNhap == tenDangNhap);
                    if (gv != null && khoaHoc.IDGiangVien == gv.IDGiangVien)
                    {
                        coQuyenHoc = true;
                    }
                }
                else if (role == "hocvien")
                {
                    hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == tenDangNhap);
                    if (hocVien != null)
                    {
                        var dangKy = db.DangKyKhoaHocs.FirstOrDefault(d => d.IDHocVien == hocVien.IDHocVien && 
                                                                           d.IDKhoaHoc == id && 
                                                                           (d.TrangThai == "Đã kích hoạt" || d.TrangThai == "Đã hoàn thành"));
                        if (dangKy != null)
                        {
                            coQuyenHoc = true;
                        }
                    }
                }
            }

            // Nếu không có quyền chính thức -> Kiểm tra xem bài yêu cầu có ChoXemThu hay không
            if (!coQuyenHoc)
            {
                if (baiId.HasValue)
                {
                    var baiThu = db.BaiHocs.Include(b => b.ChuongHoc).FirstOrDefault(b => b.IDBaiHoc == baiId.Value && b.ChuongHoc.IDKhoaHoc == id);
                    if (baiThu != null && baiThu.ChoXemThu == true)
                    {
                        isHocThu = true;
                    }
                }

                if (!isHocThu)
                {
                    if (Session["User"] == null)
                    {
                        TempData["ThongBaoDangNhap"] = "Vui lòng đăng nhập để vào học bài giảng!";
                        return RedirectToAction("DangNhap", "DangNhap");
                    }
                    TempData["ThongBao"] = "Bạn chưa đăng ký hoặc chưa hoàn tất thanh toán khóa học này!";
                    return RedirectToAction("ChiTietKhoaHoc", "KhoaHoc", new { id = id });
                }
            }

            ViewBag.IsHocThu = isHocThu;
            ViewBag.CoQuyenToanKhoa = coQuyenHoc;

            // Danh sách toàn bộ bài học theo thứ tự chương và bài
            var allBaiHocs = new List<BaiHoc>();
            var dsChuongItems = new List<ChuongHocItem>();

            // Lấy danh sách ID các bài học mà học viên đã hoàn thành (nếu đã đăng nhập học viên)
            var completedBaiIds = new List<int>();
            if (hocVien != null)
            {
                completedBaiIds = db.TienDoHocs
                                    .Where(t => t.IDHocVien == hocVien.IDHocVien && 
                                                t.DaHoanThanh == true && 
                                                t.BaiHoc.ChuongHoc.IDKhoaHoc == id)
                                    .Select(t => t.IDBaiHoc)
                                    .ToList();
            }

            var dsChuongSorted = khoaHoc.ChuongHocs.OrderBy(c => c.ThuTu).ToList();
            foreach (var ch in dsChuongSorted)
            {
                var dsBaiSorted = ch.BaiHocs.OrderBy(b => b.ThuTu).ToList();
                allBaiHocs.AddRange(dsBaiSorted);

                var chItem = new ChuongHocItem
                {
                    Chuong = ch,
                    DanhSachBai = dsBaiSorted.Select(b => new BaiHocItem
                    {
                        Bai = b,
                        DaHoanThanh = completedBaiIds.Contains(b.IDBaiHoc),
                        DangHoc = false
                    }).ToList()
                };
                dsChuongItems.Add(chItem);
            }

            if (allBaiHocs.Count == 0)
            {
                ViewBag.ThongBao = "Khóa học này hiện chưa có bài học nào.";
                return View("KhoaHocChuaCoBai", khoaHoc);
            }

            // 2. Xác định bài học hiển thị
            BaiHoc baiHienTai = null;

            if (baiId.HasValue)
            {
                baiHienTai = allBaiHocs.FirstOrDefault(b => b.IDBaiHoc == baiId.Value);
            }

            if (baiHienTai == null)
            {
                if (hocVien != null)
                {
                    // Lấy bài học gần nhất đã xem
                    var tienDoGanNhat = db.TienDoHocs
                                          .Where(t => t.IDHocVien == hocVien.IDHocVien && t.BaiHoc.ChuongHoc.IDKhoaHoc == id)
                                          .OrderByDescending(t => t.ThoiDiemXemGanNhat)
                                          .FirstOrDefault();
                    if (tienDoGanNhat != null)
                    {
                        baiHienTai = allBaiHocs.FirstOrDefault(b => b.IDBaiHoc == tienDoGanNhat.IDBaiHoc);
                    }
                }
            }

            if (baiHienTai == null)
            {
                // Mặc định lấy bài đầu tiên
                baiHienTai = allBaiHocs.FirstOrDefault();
            }

            // Nếu người dùng đang học thử mà cố tình mở bài không cho xem thử -> chặn lại
            if (isHocThu && baiHienTai.ChoXemThu != true)
            {
                TempData["ThongBao"] = "Bài học này không thuộc danh sách học thử miễn phí. Vui lòng đăng ký khóa học để tiếp tục!";
                return RedirectToAction("ChiTietKhoaHoc", "KhoaHoc", new { id = id });
            }

            // Cập nhật trạng thái 'DangHoc' trong danh sách
            foreach (var ch in dsChuongItems)
            {
                foreach (var b in ch.DanhSachBai)
                {
                    if (b.Bai.IDBaiHoc == baiHienTai.IDBaiHoc)
                    {
                        b.DangHoc = true;
                    }
                }
            }

            // 3. Ghi nhận thời điểm xem gần nhất vào TienDoHoc (nếu là học viên chính thức)
            if (hocVien != null && coQuyenHoc)
            {
                var tienDoRecord = db.TienDoHocs.FirstOrDefault(t => t.IDHocVien == hocVien.IDHocVien && t.IDBaiHoc == baiHienTai.IDBaiHoc);
                if (tienDoRecord == null)
                {
                    tienDoRecord = new TienDoHoc
                    {
                        IDHocVien = hocVien.IDHocVien,
                        IDBaiHoc = baiHienTai.IDBaiHoc,
                        DaHoanThanh = false,
                        ThoiDiemXemGanNhat = DateTime.Now
                    };
                    db.TienDoHocs.Add(tienDoRecord);
                }
                else
                {
                    tienDoRecord.ThoiDiemXemGanNhat = DateTime.Now;
                }
                db.SaveChanges();
            }

            // 4. Xác định bài trước và bài sau
            int currentIndex = allBaiHocs.FindIndex(b => b.IDBaiHoc == baiHienTai.IDBaiHoc);
            BaiHoc baiTruoc = currentIndex > 0 ? allBaiHocs[currentIndex - 1] : null;
            BaiHoc baiSau = currentIndex < allBaiHocs.Count - 1 ? allBaiHocs[currentIndex + 1] : null;

            bool daHoanThanhHienTai = completedBaiIds.Contains(baiHienTai.IDBaiHoc);

            var viewModel = new VaoHocViewModel
            {
                KhoaHoc = khoaHoc,
                BaiHocHienTai = baiHienTai,
                DanhSachChuong = dsChuongItems,
                TongSoBai = allBaiHocs.Count,
                SoBaiDaHoanThanh = completedBaiIds.Count,
                DaHoanThanhBaiHienTai = daHoanThanhHienTai,
                BaiHocTruoc = baiTruoc,
                BaiHocSau = baiSau
            };

            return View(viewModel);
        }

        // POST: HocTap/DanhDauHoanThanh (AJAX - Kiểm tra quyền và CSRF Token)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DanhDauHoanThanh(int idBaiHoc)
        {
            if (Session["User"] == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để ghi nhận tiến độ học tập!" });
            }

            string role = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";
            if (role != "hocvien")
            {
                return Json(new { success = false, message = "Chỉ học viên mới có thể ghi nhận tiến độ học tập!" });
            }

            string tenDangNhap = Session["User"].ToString();
            var hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == tenDangNhap);
            if (hocVien == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ học viên!" });
            }

            var baiHoc = db.BaiHocs.Include(b => b.ChuongHoc).FirstOrDefault(b => b.IDBaiHoc == idBaiHoc);
            if (baiHoc == null || baiHoc.ChuongHoc == null)
            {
                return Json(new { success = false, message = "Bài học không tồn tại!" });
            }

            string idKhoaHoc = baiHoc.ChuongHoc.IDKhoaHoc;

            // Kiểm tra quyền: học viên bắt buộc phải có bản ghi DangKyKhoaHoc ở trạng thái Đã kích hoạt hoặc Đã hoàn thành
            var dangKy = db.DangKyKhoaHocs.FirstOrDefault(d => d.IDHocVien == hocVien.IDHocVien && 
                                                               d.IDKhoaHoc == idKhoaHoc && 
                                                               (d.TrangThai == "Đã kích hoạt" || d.TrangThai == "Đã hoàn thành"));
            if (dangKy == null)
            {
                return Json(new { success = false, message = "Bạn chưa đăng ký khóa học này hoặc khóa học chưa được kích hoạt." });
            }

            // Tìm hoặc tạo mới bản ghi tiến độ
            var tienDo = db.TienDoHocs.FirstOrDefault(t => t.IDHocVien == hocVien.IDHocVien && t.IDBaiHoc == idBaiHoc);
            bool daHoanThanhMoi = true;
            if (tienDo == null)
            {
                tienDo = new TienDoHoc
                {
                    IDHocVien = hocVien.IDHocVien,
                    IDBaiHoc = idBaiHoc,
                    DaHoanThanh = true,
                    NgayHoanThanh = DateTime.Now,
                    ThoiDiemXemGanNhat = DateTime.Now
                };
                db.TienDoHocs.Add(tienDo);
            }
            else
            {
                tienDo.DaHoanThanh = !(tienDo.DaHoanThanh ?? false);
                daHoanThanhMoi = tienDo.DaHoanThanh.Value;
                tienDo.NgayHoanThanh = daHoanThanhMoi ? (DateTime?)DateTime.Now : null;
                tienDo.ThoiDiemXemGanNhat = DateTime.Now;
            }

            db.SaveChanges();

            // Tính toán lại tiến độ toàn khóa
            int tongSoBai = db.BaiHocs.Count(b => b.ChuongHoc.IDKhoaHoc == idKhoaHoc);
            int soBaiHoanThanh = db.TienDoHocs.Count(t => t.IDHocVien == hocVien.IDHocVien && 
                                                         t.DaHoanThanh == true && 
                                                         t.BaiHoc.ChuongHoc.IDKhoaHoc == idKhoaHoc);

            int phanTram = tongSoBai > 0 ? (int)Math.Round((double)soBaiHoanThanh / tongSoBai * 100) : 0;

            // Nếu hoàn thành 100% -> Cập nhật trạng thái khóa học đã hoàn thành
            if (phanTram >= 100)
            {
                dangKy.TrangThai = "Đã hoàn thành";
                if (!dangKy.NgayHoanThanh.HasValue)
                {
                    dangKy.NgayHoanThanh = DateTime.Now;
                }
                db.SaveChanges();
            }
            else if (dangKy.TrangThai == "Đã hoàn thành")
            {
                // Nếu chưa đủ 100% (ví dụ bỏ tick hoặc giảng viên vừa thêm bài mới) -> hoàn lại trạng thái Đã kích hoạt
                dangKy.TrangThai = "Đã kích hoạt";
                db.SaveChanges();
            }

            return Json(new
            {
                success = true,
                daHoanThanh = daHoanThanhMoi,
                soBaiHoanThanh = soBaiHoanThanh,
                tongSoBai = tongSoBai,
                phanTram = phanTram,
                message = daHoanThanhMoi ? "Đã đánh dấu hoàn thành bài học!" : "Đã hủy đánh dấu hoàn thành bài học!"
            });
        }

        // GET: HocTap/ThongTinCaNhan
        public ActionResult ThongTinCaNhan()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string username = Session["User"].ToString();
            var hocVien = db.HocViens.Include(h => h.TaiKhoan).FirstOrDefault(h => h.IDTenDangNhap == username);
            if (hocVien == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(hocVien);
        }

        // POST: HocTap/ThongTinCaNhan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThongTinCaNhan(string TenHV, string Email, string SDT, string DiaChi)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string username = Session["User"].ToString();
            var hocVien = db.HocViens.Include(h => h.TaiKhoan).FirstOrDefault(h => h.IDTenDangNhap == username);
            if (hocVien != null)
            {
                hocVien.TenHV = TenHV;
                hocVien.DiaChi = DiaChi;
                if (hocVien.TaiKhoan != null)
                {
                    hocVien.TaiKhoan.Email = Email;
                    hocVien.TaiKhoan.SDT = SDT;
                }

                db.SaveChanges();
                Session["TenHienThi"] = TenHV;
                TempData["ThongBao"] = "Cập nhật thông tin cá nhân thành công!";
            }

            return RedirectToAction("ThongTinCaNhan");
        }

        // GET: HocTap/XemDiem
        public ActionResult XemDiem()
        {
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Vui lòng đăng nhập để xem bảng điểm!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string tenDangNhap = Session["User"].ToString();
            var hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == tenDangNhap);
            if (hocVien == null)
            {
                TempData["ThongBaoDangNhap"] = "Không tìm thấy hồ sơ học viên!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var dsDangKy = db.DangKyKhoaHocs
                             .Include(d => d.KhoaHoc)
                             .Include(d => d.KhoaHoc.GiangVien)
                             .Include(d => d.DiemKhoaHocs)
                             .Where(d => d.IDHocVien == hocVien.IDHocVien &&
                                        (d.TrangThai == "Đã kích hoạt" || d.TrangThai == "Đã hoàn thành"))
                             .OrderByDescending(d => d.NgayDangKy)
                             .ToList();

            var bangDiem = new List<DiemViewModel>();

            foreach (var dk in dsDangKy)
            {
                var kh = dk.KhoaHoc;
                if (kh == null) continue;

                int tongSoBai = db.BaiHocs.Count(b => b.ChuongHoc.IDKhoaHoc == kh.IDKhoaHoc);
                int soBaiHoanThanh = db.TienDoHocs.Count(t => t.IDHocVien == hocVien.IDHocVien &&
                                                             t.DaHoanThanh == true &&
                                                             t.BaiHoc.ChuongHoc.IDKhoaHoc == kh.IDKhoaHoc);
                int phanTram = tongSoBai > 0 ? (int)Math.Round((double)soBaiHoanThanh / tongSoBai * 100) : 0;
                var diemItem = dk.DiemKhoaHocs != null ? dk.DiemKhoaHocs.FirstOrDefault() : null;

                bangDiem.Add(new DiemViewModel
                {
                    IDDangKy = dk.IDDangKy,
                    IDHocVien = hocVien.IDHocVien,
                    TenHocVien = hocVien.TenHV,
                    TenDangNhap = hocVien.IDTenDangNhap,
                    IDKhoaHoc = kh.IDKhoaHoc,
                    TenKhoaHoc = kh.TenKhoaHoc,
                    TongSoBaiHoc = tongSoBai,
                    SoBaiDaHoc = soBaiHoanThanh,
                    TienDoPhanTram = phanTram,
                    Diem = diemItem != null ? diemItem.Diem : null,
                    NhanXet = diemItem != null ? diemItem.NhanXet : null,
                    NgayCapNhat = diemItem != null ? diemItem.NgayCapNhat : null,
                    TrangThaiDangKy = dk.TrangThai
                });
            }

            ViewBag.TenHocVien = hocVien.TenHV;
            return View(bangDiem);
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