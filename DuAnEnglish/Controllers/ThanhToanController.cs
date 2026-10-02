using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class ThanhToanController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: ThanhToan/DanhSachHoaDon
        public ActionResult DanhSachHoaDon()
        {
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Vui lòng đăng nhập để xem danh sách hóa đơn!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string tenDangNhap = Session["User"].ToString();
            var hoaDons = db.ThanhToans
                            .Include(t => t.KhoaHoc)
                            .Include(t => t.DangKyKhoaHoc)
                            .Where(t => t.TenDangNhap == tenDangNhap)
                            .OrderByDescending(t => t.IDThanhToan)
                            .ToList();

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(hoaDons);
        }

        // GET: ThanhToan/HoaDon/5
        public ActionResult HoaDon(int id)
        {
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Vui lòng đăng nhập để thực hiện thanh toán!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string tenDangNhap = Session["User"].ToString();
            var hoaDon = db.ThanhToans
                           .Include(h => h.KhoaHoc)
                           .FirstOrDefault(h => h.IDThanhToan == id);

            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn cần thanh toán.";
                return RedirectToAction("DanhSachHoaDon");
            }

            // Kiểm tra quyền sở hữu hóa đơn
            string role = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";
            if (hoaDon.TenDangNhap != tenDangNhap && role != "admin")
            {
                TempData["ThongBao"] = "Bạn không có quyền truy cập vào hóa đơn này!";
                return RedirectToAction("DanhSachHoaDon");
            }

            var hocVien = db.HocViens.FirstOrDefault(hv => hv.IDTenDangNhap == hoaDon.TenDangNhap);
            ViewBag.TenHocVien = hocVien != null ? hocVien.TenHV : hoaDon.TenDangNhap;
            ViewBag.Email = hocVien != null && hocVien.TaiKhoan != null ? hocVien.TaiKhoan.Email : "";
            ViewBag.SDT = hocVien != null && hocVien.TaiKhoan != null ? hocVien.TaiKhoan.SDT : "";

            return View(hoaDon);
        }

        // POST: ThanhToan/ThanhToanVNPay
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToanVNPay(int id)
        {
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Vui lòng đăng nhập để thanh toán!";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var hoaDon = db.ThanhToans.FirstOrDefault(h => h.IDThanhToan == id && (h.TrangThai == "Chưa thanh toán" || h.TrangThai == "Thanh toán thất bại"));
            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Hóa đơn không tồn tại hoặc đã được xử lý.";
                return RedirectToAction("DanhSachHoaDon");
            }

            // Kiểm tra quyền sở hữu hóa đơn
            string tenDangNhap = Session["User"].ToString();
            string role = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";
            if (hoaDon.TenDangNhap != tenDangNhap && role != "admin")
            {
                TempData["ThongBao"] = "Bạn không có quyền thực hiện thanh toán hóa đơn của người khác!";
                return RedirectToAction("DanhSachHoaDon");
            }

            try
            {
                string vnp_Url = ConfigurationManager.AppSettings["Vnp_Url"];
                string vnp_TmnCode = ConfigurationManager.AppSettings["Vnp_TmnCode"];
                string vnp_HashSecret = ConfigurationManager.AppSettings["Vnp_HashSecret"];

                if (string.IsNullOrWhiteSpace(vnp_Url) || string.IsNullOrWhiteSpace(vnp_TmnCode) || string.IsNullOrWhiteSpace(vnp_HashSecret))
                {
                    TempData["ThongBao"] = "Lỗi cấu hình cổng thanh toán VNPay trên máy chủ. Vui lòng liên hệ quản trị viên.";
                    return RedirectToAction("HoaDon", new { id = id });
                }

                string vnp_Returnurl = ConfigurationManager.AppSettings["Vnp_ReturnUrl"];
                if (string.IsNullOrEmpty(vnp_Returnurl))
                {
                    vnp_Returnurl = Url.Action("ReturnVnpay", "ThanhToan", null, Request.Url.Scheme);
                }

                long amount = (long)((hoaDon.SoTien ?? 0) * 100); // VNPay yêu cầu nhân 100

                VnPayLibrary vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", amount.ToString());
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", string.Format("Thanh toan khoa hoc #{0}", hoaDon.IDKhoaHoc));
                vnpay.AddRequestData("vnp_OrderType", "education");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", hoaDon.IDThanhToan.ToString());

                // Chỉ cập nhật phương thức thanh toán dự kiến, KHÔNG cập nhật ngày thanh toán thành công tại thời điểm khởi tạo
                hoaDon.PhuongThucTT = "VNPAY";
                db.SaveChanges();

                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Lỗi khởi tạo cổng VNPay: " + ex.Message;
                return RedirectToAction("HoaDon", new { id = id });
            }
        }

        // GET: ThanhToan/ReturnVnpay (Callback từ VNPay - Đảm bảo Idempotent)
        public ActionResult ReturnVnpay()
        {
            var vnpay = new VnPayLibrary();
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, Request.QueryString[key]);
                }
            }

            string vnp_HashSecret = ConfigurationManager.AppSettings["Vnp_HashSecret"];
            if (string.IsNullOrWhiteSpace(vnp_HashSecret))
            {
                TempData["ThongBao"] = "Lỗi cấu hình hệ thống: Không tìm thấy khóa bí mật VNPay.";
                return RedirectToAction("DanhSachHoaDon");
            }

            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
            bool isValid = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

            if (!isValid)
            {
                TempData["ThongBao"] = "Chữ ký bảo mật VNPay không hợp lệ (Sai checksum bảo mật)!";
                return RedirectToAction("DanhSachHoaDon");
            }

            string vnp_TxnRef = Request.QueryString["vnp_TxnRef"];
            int idThanhToan;
            if (!int.TryParse(vnp_TxnRef, out idThanhToan))
            {
                TempData["ThongBao"] = "Mã hóa đơn phản hồi từ VNPay không hợp lệ.";
                return RedirectToAction("DanhSachHoaDon");
            }

            var hoaDon = db.ThanhToans.Include(t => t.DangKyKhoaHoc).FirstOrDefault(h => h.IDThanhToan == idThanhToan);
            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn tương ứng.";
                return RedirectToAction("DanhSachHoaDon");
            }

            // Chống callback lặp: Nếu hóa đơn đã được thanh toán thành công thì không xử lý lại
            if (hoaDon.TrangThai == "Đã thanh toán")
            {
                TempData["ThongBao"] = "Hóa đơn này đã được thanh toán và kích hoạt thành công trước đó!";
                return RedirectToAction("VaoHoc", "HocTap", new { id = hoaDon.IDKhoaHoc });
            }

            string responseCode = Request.QueryString["vnp_ResponseCode"];
            string transactionNo = Request.QueryString["vnp_TransactionNo"];
            string vnp_Amount = Request.QueryString["vnp_Amount"];

            // Kiểm tra số tiền phản hồi từ VNPay bắt buộc phải đúng định dạng và khớp chính xác 100% với hóa đơn
            long vnpAmountLong;
            long expectedAmount = (long)((hoaDon.SoTien ?? 0) * 100);
            if (!long.TryParse(vnp_Amount, out vnpAmountLong) || vnpAmountLong != expectedAmount)
            {
                hoaDon.TrangThai = "Thanh toán thất bại";
                db.SaveChanges();
                TempData["ThongBao"] = "Số tiền thanh toán từ cổng VNPay không hợp lệ hoặc không khớp với hóa đơn.";
                return RedirectToAction("DanhSachHoaDon");
            }

            // 1. Lưu log giao dịch VNPAY (chống ghi đè trùng mã giao dịch)
            try
            {
                bool logExists = !string.IsNullOrEmpty(transactionNo) && 
                                 db.GiaoDichVNPAYs.Any(g => g.MaGiaoDich == transactionNo && g.IDThanhToan == hoaDon.IDThanhToan);
                if (!logExists)
                {
                    db.GiaoDichVNPAYs.Add(new GiaoDichVNPAY
                    {
                        IDThanhToan = hoaDon.IDThanhToan,
                        MaGiaoDich = transactionNo,
                        NgayGiaoDich = DateTime.Now,
                        SoTien = hoaDon.SoTien,
                        TrangThai = responseCode == "00" ? "Thành công" : "Thất bại",
                        NoiDung = string.Format("Thanh toán cho hóa đơn #{0} - Khóa học {1}", hoaDon.IDThanhToan, hoaDon.IDKhoaHoc),
                        PhanHoiVNPAY = responseCode
                    });
                }
            }
            catch { }

            // 2. Xử lý trạng thái thanh toán và kích hoạt khóa học
            if (responseCode == "00")
            {
                hoaDon.TrangThai = "Đã thanh toán";
                hoaDon.NgayThanhToan = DateTime.Now;
                hoaDon.NgayXacNhan = DateTime.Now;

                // Tự động kích hoạt quyền học
                if (hoaDon.IDDangKy.HasValue)
                {
                    var dangKy = db.DangKyKhoaHocs.Find(hoaDon.IDDangKy.Value);
                    if (dangKy != null)
                    {
                        dangKy.TrangThai = "Đã kích hoạt";
                    }
                }
                else
                {
                    // Tìm kiếm bản ghi đăng ký theo học viên và khóa học
                    var hocVien = db.HocViens.FirstOrDefault(h => h.IDTenDangNhap == hoaDon.TenDangNhap);
                    if (hocVien != null)
                    {
                        var dangKy = db.DangKyKhoaHocs.FirstOrDefault(d => d.IDHocVien == hocVien.IDHocVien && d.IDKhoaHoc == hoaDon.IDKhoaHoc);
                        if (dangKy != null)
                        {
                            dangKy.TrangThai = "Đã kích hoạt";
                        }
                    }
                }

                db.SaveChanges();

                TempData["ThongBao"] = "Thanh toán thành công qua VNPay! Khóa học của bạn đã được kích hoạt, hãy bắt đầu học ngay bây giờ.";
                return RedirectToAction("VaoHoc", "HocTap", new { id = hoaDon.IDKhoaHoc });
            }
            else
            {
                hoaDon.TrangThai = "Thanh toán thất bại";
                db.SaveChanges();

                TempData["ThongBao"] = string.Format("Giao dịch thanh toán không thành công hoặc đã bị hủy (Mã phản hồi: {0}). Bạn có thể thanh toán lại.", responseCode);
                return RedirectToAction("DanhSachHoaDon");
            }
        }

        // POST: ThanhToan/XoaHoaDon - Hủy hóa đơn an toàn với CSRF protection
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaHoaDon(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string tenDangNhap = Session["User"].ToString();
            string role = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";
            var hoaDon = db.ThanhToans.FirstOrDefault(h => h.IDThanhToan == id);

            if (hoaDon != null)
            {
                if (hoaDon.TenDangNhap != tenDangNhap && role != "admin")
                {
                    TempData["ThongBao"] = "Bạn không có quyền thao tác trên hóa đơn này!";
                    return RedirectToAction("DanhSachHoaDon");
                }

                if (hoaDon.TrangThai == "Đã thanh toán")
                {
                    TempData["ThongBao"] = "Không thể xóa hoặc hủy hóa đơn đã thanh toán thành công!";
                    return RedirectToAction("DanhSachHoaDon");
                }

                if (hoaDon.IDDangKy.HasValue)
                {
                    var dangKy = db.DangKyKhoaHocs.Find(hoaDon.IDDangKy.Value);
                    if (dangKy != null && dangKy.TrangThai == "Chưa kích hoạt")
                    {
                        db.DangKyKhoaHocs.Remove(dangKy);
                    }
                }
                db.ThanhToans.Remove(hoaDon);
                db.SaveChanges();
                TempData["ThongBao"] = "Đã hủy hóa đơn thành công.";
            }

            return RedirectToAction("DanhSachHoaDon");
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
