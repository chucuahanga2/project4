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

            var hoaDon = db.ThanhToans.FirstOrDefault(h => h.IDThanhToan == id && h.TrangThai == "Chưa thanh toán");
            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Hóa đơn không tồn tại hoặc đã được xử lý.";
                return RedirectToAction("DanhSachHoaDon");
            }

            try
            {
                VnPayLibrary vnpay = new VnPayLibrary();
                string vnp_Returnurl = ConfigurationManager.AppSettings["Vnp_ReturnUrl"];
                if (string.IsNullOrEmpty(vnp_Returnurl))
                {
                    vnp_Returnurl = Url.Action("ReturnVnpay", "ThanhToan", null, Request.Url.Scheme);
                }

                string vnp_Url = ConfigurationManager.AppSettings["Vnp_Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                string vnp_TmnCode = ConfigurationManager.AppSettings["Vnp_TmnCode"] ?? "2QXUI4J4";
                string vnp_HashSecret = ConfigurationManager.AppSettings["Vnp_HashSecret"] ?? "RAOICV25ENA20VTXBWD7KEUMFSOYSRS5";

                long amount = (long)((hoaDon.SoTien ?? 0) * 100); // VNPay yêu cầu nhân 100

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

                // Cập nhật phương thức
                hoaDon.PhuongThucTT = "VNPAY";
                hoaDon.NgayThanhToan = DateTime.Now;
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

        // GET: ThanhToan/ReturnVnpay (Callback từ VNPay)
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

            string vnp_HashSecret = ConfigurationManager.AppSettings["Vnp_HashSecret"] ?? "RAOICV25ENA20VTXBWD7KEUMFSOYSRS5";
            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
            bool isValid = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

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

            string responseCode = Request.QueryString["vnp_ResponseCode"];
            string transactionNo = Request.QueryString["vnp_TransactionNo"];

            // 1. Lưu log giao dịch VNPAY
            try
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
            catch { }

            // 2. Xử lý trạng thái thanh toán và kích hoạt khóa học
            if (isValid && responseCode == "00")
            {
                hoaDon.TrangThai = "Đã thanh toán";
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

                TempData["ThongBao"] = string.Format("Giao dịch thanh toán không thành công hoặc đã bị hủy (Mã phản hồi: {0}). Bạn có thể thử lại.", responseCode);
                return RedirectToAction("DanhSachHoaDon");
            }
        }

        // Hủy hóa đơn chưa thanh toán
        public ActionResult XoaHoaDon(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string tenDangNhap = Session["User"].ToString();
            var hoaDon = db.ThanhToans.FirstOrDefault(h => h.IDThanhToan == id && h.TenDangNhap == tenDangNhap && h.TrangThai == "Chưa thanh toán");

            if (hoaDon != null)
            {
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
