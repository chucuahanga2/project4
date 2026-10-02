using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.ViewModels;

namespace DuAnEnglish.Controllers
{
    public class KhoaHocController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: KhoaHoc/Index
        public ActionResult Index(string danhmuc = "all", string category = null, string search = "", string searchString = null, string gia = "all", string priceFilter = null, string sort = "newest")
        {
            return RedirectToAction("KhoaHoc", new { danhmuc, category, search, searchString, gia, priceFilter, sort });
        }

        // GET: KhoaHoc/KhoaHoc
        public ActionResult KhoaHoc(string danhmuc = "all", string category = null, string search = "", string searchString = null, string gia = "all", string priceFilter = null, string sort = "newest")
        {
            // Hỗ trợ các tên tham số đồng nghĩa từ URL/Form
            if (!string.IsNullOrEmpty(category) && (string.IsNullOrEmpty(danhmuc) || danhmuc == "all"))
            {
                danhmuc = category;
            }
            if (!string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(search))
            {
                search = searchString;
            }
            if (!string.IsNullOrEmpty(priceFilter) && (string.IsNullOrEmpty(gia) || gia == "all"))
            {
                gia = priceFilter;
            }

            // Chỉ lấy các khóa học công khai (không ở trạng thái Ẩn)
            var query = db.KhoaHocs
                          .Include(k => k.DanhMucKhoaHoc)
                          .Include(k => k.GiangVien)
                          .Where(k => k.TrangThai != "Ẩn" || k.TrangThai == null);

            // 1. Lọc theo danh mục
            if (!string.IsNullOrEmpty(danhmuc) && danhmuc != "all")
            {
                int idDanhMuc;
                if (int.TryParse(danhmuc, out idDanhMuc))
                {
                    query = query.Where(k => k.IDDanhMuc == idDanhMuc);
                }
                else
                {
                    query = query.Where(k => k.DanhMuc == danhmuc || (k.DanhMucKhoaHoc != null && k.DanhMucKhoaHoc.TenDanhMuc == danhmuc));
                }
            }

            // 2. Tìm kiếm theo từ khóa
            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(k => k.TenKhoaHoc.ToLower().Contains(kw) || (k.MoTa != null && k.MoTa.ToLower().Contains(kw)));
            }

            // 3. Lọc theo mức giá
            if (gia == "free")
            {
                query = query.Where(k => k.HocPhi == null || k.HocPhi == 0);
            }
            else if (gia == "paid")
            {
                query = query.Where(k => k.HocPhi != null && k.HocPhi > 0);
            }

            // 4. Sắp xếp
            if (sort == "price_asc")
            {
                query = query.OrderBy(k => k.HocPhi ?? 0);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(k => k.HocPhi ?? 0);
            }
            else if (sort == "name")
            {
                query = query.OrderBy(k => k.TenKhoaHoc);
            }
            else
            {
                // Mặc định mới nhất
                query = query.OrderByDescending(k => k.NgayTao);
            }

            var dsKhoaHoc = query.ToList();

            ViewBag.DanhSachDanhMuc = db.DanhMucKhoaHocs.Where(d => d.TrangThai == "Hoạt động").OrderBy(d => d.ThuTu).ToList();
            ViewBag.CurrentDanhMuc = danhmuc;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentGia = gia;
            ViewBag.CurrentSort = sort;

            return View(dsKhoaHoc);
        }

        // GET: KhoaHoc/ChiTietKhoaHoc/5
        public ActionResult ChiTietKhoaHoc(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("KhoaHoc");
            }

            var khoaHoc = db.KhoaHocs
                            .Include(k => k.DanhMucKhoaHoc)
                            .Include(k => k.GiangVien)
                            .Include(k => k.ChuongHocs.Select(c => c.BaiHocs))
                            .FirstOrDefault(k => k.IDKhoaHoc == id);

            if (khoaHoc == null)
            {
                return HttpNotFound("Không tìm thấy khóa học yêu cầu.");
            }

            // Kiểm tra trạng thái khóa học ẩn: chỉ Admin hoặc Giảng viên sở hữu mới được xem
            if (khoaHoc.TrangThai == "Ẩn")
            {
                bool coQuyenXem = false;
                if (Session["User"] != null && Session["Role"] != null)
                {
                    string role = Session["Role"].ToString().ToLower();
                    if (role == "admin")
                    {
                        coQuyenXem = true;
                    }
                    else if (role == "giangvien")
                    {
                        string username = Session["User"].ToString();
                        var gv = db.GiangViens.FirstOrDefault(g => g.IDTenDangNhap == username);
                        if (gv != null && khoaHoc.IDGiangVien == gv.IDGiangVien)
                        {
                            coQuyenXem = true;
                        }
                    }
                }
                if (!coQuyenXem)
                {
                    return HttpNotFound("Khóa học hiện không khả dụng hoặc đang bị ẩn.");
                }
            }

            // Sắp xếp chương và bài
            var dsChuongItem = new List<ChuongHocItem>();
            int tongSoBai = 0;
            int tongThoiLuong = 0;

            var dsChuong = khoaHoc.ChuongHocs.OrderBy(c => c.ThuTu).ToList();
            foreach (var ch in dsChuong)
            {
                var dsBai = ch.BaiHocs.OrderBy(b => b.ThuTu).ToList();
                tongSoBai += dsBai.Count;
                tongThoiLuong += dsBai.Sum(b => b.ThoiLuong ?? 0);

                var chItem = new ChuongHocItem
                {
                    Chuong = ch,
                    DanhSachBai = dsBai.Select(b => new BaiHocItem
                    {
                        Bai = b,
                        DaHoanThanh = false,
                        DangHoc = false
                    }).ToList()
                };
                dsChuongItem.Add(chItem);
            }

            // Kiểm tra trạng thái học viên đăng ký
            bool daDangKy = false;
            bool daKichHoat = false;
            int phanTramTienDo = 0;

            if (Session["User"] != null && Session["Role"] != null && Session["Role"].ToString() == "hocvien")
            {
                string tenDangNhap = Session["User"].ToString();
                var hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == tenDangNhap);
                if (hocVien != null)
                {
                    var dangKy = db.DangKyKhoaHocs.FirstOrDefault(d => d.IDHocVien == hocVien.IDHocVien && d.IDKhoaHoc == id);
                    if (dangKy != null)
                    {
                        daDangKy = true;
                        if (dangKy.TrangThai == "Đã kích hoạt" || dangKy.TrangThai == "Đã hoàn thành")
                        {
                            daKichHoat = true;
                            // Tính % tiến độ
                            int soBaiHoanThanh = db.TienDoHocs.Count(t => t.IDHocVien == hocVien.IDHocVien &&
                                                                        t.DaHoanThanh == true &&
                                                                        t.BaiHoc.ChuongHoc.IDKhoaHoc == id);
                            phanTramTienDo = tongSoBai > 0 ? (int)Math.Round((double)soBaiHoanThanh / tongSoBai * 100) : 0;
                        }
                    }
                }
            }

            // Khóa học liên quan
            var lienQuan = db.KhoaHocs
                             .Where(k => k.IDKhoaHoc != id && (k.TrangThai == "Hiển thị" || k.TrangThai == null) && k.IDDanhMuc == khoaHoc.IDDanhMuc)
                             .Take(3)
                             .ToList();

            var viewModel = new ChiTietKhoaHocViewModel
            {
                KhoaHoc = khoaHoc,
                DanhSachChuong = dsChuongItem,
                TongSoBaiHoc = tongSoBai,
                TongThoiLuong = tongThoiLuong,
                DaDangKy = daDangKy,
                DaKichHoat = daKichHoat,
                PhanTramTienDo = phanTramTienDo,
                KhoaHocLienQuan = lienQuan
            };

            return View(viewModel);
        }

        // GET: KhoaHoc/DangKyKhoaHoc/5 - Trang xác nhận thông tin đăng ký
        [HttpGet]
        public ActionResult DangKyKhoaHoc(string id)
        {
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Vui lòng đăng nhập để đăng ký khóa học!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string role = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";
            if (role != "hocvien")
            {
                TempData["ThongBao"] = "Tài khoản hiện tại không phải học viên. Vui lòng đăng nhập bằng tài khoản học viên để đăng ký!";
                return RedirectToAction("ChiTietKhoaHoc", new { id = id });
            }

            string tenDangNhap = Session["User"].ToString();
            var hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == tenDangNhap);
            if (hocVien == null)
            {
                TempData["ThongBaoDangNhap"] = "Không tìm thấy hồ sơ học viên!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var khoaHoc = db.KhoaHocs.Include(k => k.GiangVien).Include(k => k.DanhMucKhoaHoc).FirstOrDefault(k => k.IDKhoaHoc == id);
            if (khoaHoc == null)
            {
                return HttpNotFound("Không tìm thấy khóa học.");
            }

            if (khoaHoc.TrangThai == "Ẩn")
            {
                TempData["ThongBao"] = "Khóa học hiện đang tạm ẩn, không thể đăng ký.";
                return RedirectToAction("KhoaHoc");
            }

            // Kiểm tra đã đăng ký trước đó chưa
            var dangKyTonTai = db.DangKyKhoaHocs.FirstOrDefault(d => d.IDHocVien == hocVien.IDHocVien && d.IDKhoaHoc == id);
            if (dangKyTonTai != null)
            {
                if (dangKyTonTai.TrangThai == "Đã kích hoạt" || dangKyTonTai.TrangThai == "Đã hoàn thành")
                {
                    TempData["ThongBao"] = "Bạn đã đăng ký và kích hoạt khóa học này. Hãy tiếp tục học tập!";
                    return RedirectToAction("VaoHoc", "HocTap", new { id = id });
                }
                else
                {
                    // Tìm hóa đơn chưa thanh toán tương ứng
                    var hoaDonCu = db.ThanhToans.FirstOrDefault(t => t.IDDangKy == dangKyTonTai.IDDangKy && t.TrangThai == "Chưa thanh toán");
                    if (hoaDonCu != null)
                    {
                        TempData["ThongBao"] = "Bạn đã có hóa đơn chờ thanh toán cho khóa học này. Vui lòng hoàn tất thanh toán!";
                        return RedirectToAction("HoaDon", "ThanhToan", new { id = hoaDonCu.IDThanhToan });
                    }
                }
            }

            ViewBag.TenHocVien = hocVien.TenHV;
            return View(khoaHoc);
        }

        // POST: KhoaHoc/DangKyKhoaHoc - Xử lý tạo đăng ký an toàn chống CSRF và trùng lặp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKyKhoaHoc(string id, string idKhoaHoc = null)
        {
            if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(idKhoaHoc))
            {
                id = idKhoaHoc;
            }

            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Vui lòng đăng nhập để đăng ký khóa học!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string role = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";
            if (role != "hocvien")
            {
                TempData["ThongBao"] = "Tài khoản hiện tại không phải học viên. Vui lòng đăng nhập bằng tài khoản học viên để đăng ký!";
                return RedirectToAction("ChiTietKhoaHoc", new { id = id });
            }

            string tenDangNhap = Session["User"].ToString();
            var hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == tenDangNhap);
            if (hocVien == null)
            {
                TempData["ThongBaoDangNhap"] = "Không tìm thấy hồ sơ học viên!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == id);
            if (khoaHoc == null)
            {
                return HttpNotFound("Không tìm thấy khóa học.");
            }

            if (khoaHoc.TrangThai == "Ẩn")
            {
                TempData["ThongBao"] = "Khóa học hiện đang tạm ẩn, không thể đăng ký.";
                return RedirectToAction("KhoaHoc");
            }

            // 1. Kiểm tra đã có bản ghi đăng ký chưa (ngăn ngừa double click hoặc gửi lặp)
            var dangKyTonTai = db.DangKyKhoaHocs.FirstOrDefault(d => d.IDHocVien == hocVien.IDHocVien && d.IDKhoaHoc == id);
            if (dangKyTonTai != null)
            {
                if (dangKyTonTai.TrangThai == "Đã kích hoạt" || dangKyTonTai.TrangThai == "Đã hoàn thành")
                {
                    TempData["ThongBao"] = "Bạn đã đăng ký và kích hoạt khóa học này. Hãy tiếp tục học tập!";
                    return RedirectToAction("VaoHoc", "HocTap", new { id = id });
                }
                else
                {
                    var hoaDonCho = db.ThanhToans.FirstOrDefault(t => t.IDDangKy == dangKyTonTai.IDDangKy && t.TrangThai == "Chưa thanh toán");
                    if (hoaDonCho != null)
                    {
                        TempData["ThongBao"] = "Khóa học đang chờ thanh toán. Vui lòng hoàn tất thanh toán để vào học!";
                        return RedirectToAction("HoaDon", "ThanhToan", new { id = hoaDonCho.IDThanhToan });
                    }
                    return RedirectToAction("DanhSachHoaDon", "ThanhToan");
                }
            }

            decimal hocPhi = khoaHoc.HocPhi ?? 0;

            // 2. Nếu là khóa học MIỄN PHÍ -> Kích hoạt ngay lập tức
            if (hocPhi <= 0)
            {
                var dangKyMoi = new DangKyKhoaHoc
                {
                    IDHocVien = hocVien.IDHocVien,
                    IDKhoaHoc = id,
                    NgayDangKy = DateTime.Now,
                    TrangThai = "Đã kích hoạt"
                };
                db.DangKyKhoaHocs.Add(dangKyMoi);
                db.SaveChanges();

                TempData["ThongBao"] = string.Format("Đăng ký thành công khóa học miễn phí '{0}'! Chúc bạn học tập tốt.", khoaHoc.TenKhoaHoc);
                return RedirectToAction("VaoHoc", "HocTap", new { id = id });
            }

            // 3. Nếu là khóa học CÓ PHÍ -> Tạo đăng ký chờ kích hoạt + tạo hóa đơn thanh toán duy nhất
            var dangKyChoTT = new DangKyKhoaHoc
            {
                IDHocVien = hocVien.IDHocVien,
                IDKhoaHoc = id,
                NgayDangKy = DateTime.Now,
                TrangThai = "Chưa kích hoạt"
            };
            db.DangKyKhoaHocs.Add(dangKyChoTT);
            db.SaveChanges();

            var hoaDon = new ThanhToan
            {
                IDDangKy = dangKyChoTT.IDDangKy,
                IDKhoaHoc = id,
                TenDangNhap = tenDangNhap,
                SoTien = hocPhi,
                PhuongThucTT = "VNPAY",
                NgayThanhToan = DateTime.Now,
                TrangThai = "Chưa thanh toán"
            };
            db.ThanhToans.Add(hoaDon);
            db.SaveChanges();

            TempData["ThongBao"] = "Đăng ký thành công! Vui lòng hoàn tất thanh toán để bắt đầu học.";
            return RedirectToAction("HoaDon", "ThanhToan", new { id = hoaDon.IDThanhToan });
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