using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;
using System.Security.Cryptography;
using System.Text;

namespace DuAnEnglish.Controllers
{
    public class ThanhToanController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: ThanhToan
        public ActionResult DanhSachHoaDon()
        {
            string tenDangNhap = Session["User"]?.ToString();
            if (tenDangNhap == null)
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để đăng ký khóa học";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var hoaDons = db.ThanhToans
                            .Where(t => t.TenDangNhap == tenDangNhap)
                            .ToList();

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            return View(hoaDons);
        }

        public ActionResult XoaHoaDon(int id)
        {
            var hoaDon = db.ThanhToans.FirstOrDefault(h => h.IDThanhToan == id && h.TrangThai == "Chưa thanh toán");

            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Không thể xóa vì đã được xử lý.";
                return RedirectToAction("DanhSachHoaDon");
            }

            db.ThanhToans.Remove(hoaDon);
            db.SaveChanges();

            TempData["ThongBao"] = "Xóa thành công.";
            return RedirectToAction("DanhSachHoaDon");
        }

        public ActionResult HoaDon(int id)
        {
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để thực hiện thanh toán.";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var hoaDon = db.ThanhToans.FirstOrDefault(h => h.IDThanhToan == id && h.TrangThai == "Chưa thanh toán");
            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn cần thanh toán.";
                return RedirectToAction("DanhSachHoaDon");
            }

            var hocVien = db.HocViens.FirstOrDefault(hv => hv.IDTenDangNhap == hoaDon.TenDangNhap);
            ViewBag.TenHocVien = hocVien != null ? hocVien.TenHV : "";

            var khoaHoc = db.KhoaHocs.FirstOrDefault(kh => kh.IDKhoaHoc == hoaDon.IDKhoaHoc);
            ViewBag.TenKhoaHoc = khoaHoc != null ? khoaHoc.TenKhoaHoc : "";

            var lopHoc = db.LopHocs.FirstOrDefault(lh => lh.IDLopHoc == hoaDon.IDLopHoc);
            ViewBag.TenLop = lopHoc != null ? lopHoc.TenLop : "";

            return View(hoaDon);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult HoaDon(int idThanhToan, string phuongThucTT, string ngayThanhToan)
        //{
        //    var thanhToan = db.ThanhToans.FirstOrDefault(t => t.IDThanhToan == idThanhToan);
        //    if (thanhToan == null)
        //    {
        //        TempData["ThongBao"] = "Không tìm thấy hóa đơn.";
        //        return RedirectToAction("DanhSachHoaDon");
        //    }

        //    DateTime parsedNgayThanhToan;
        //    if (!DateTime.TryParse(ngayThanhToan, out parsedNgayThanhToan))
        //    {
        //        parsedNgayThanhToan = DateTime.Now;
        //    }

        //    var lopHoc = db.LopHocs.FirstOrDefault(l => l.IDLopHoc == thanhToan.IDLopHoc);
        //    if (lopHoc == null)
        //    {
        //        TempData["ThongBao"] = "Không tìm thấy lớp học cho hóa đơn này.";
        //        return RedirectToAction("DanhSachHoaDon");
        //    }

        //    if (phuongThucTT == "Thanh toán trực tiếp")
        //    {
        //        thanhToan.PhuongThucTT = "Thanh toán trực tiếp";
        //        thanhToan.NgayThanhToan = parsedNgayThanhToan;
        //        thanhToan.TrangThai = "Chờ xử lý";
        //        thanhToan.NgayXacNhan = null;

        //        if (lopHoc.Slot > 0)
        //        {
        //            lopHoc.Slot -= 1;
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            TempData["ThongBao"] = "Lớp học đã hết chỗ, không thể xác nhận thanh toán.";
        //            return RedirectToAction("DanhSachHoaDon");
        //        }

        //        db.SaveChanges();

        //        TempData["ThongBao"] = "Đã gửi yêu cầu thanh toán trực tiếp, vui lòng chờ xử lý.";
        //        return RedirectToAction("DanhSachHoaDon");
        //    }
        //    else if (phuongThucTT == "Thanh toán qua VNPAY")
        //    {
        //        string returnUrl = Url.Action("VnpReturn", "ThanhToan", null, Request.Url.Scheme);
        //        string vnpayUrl = CreateVnpayPaymentUrl(thanhToan, returnUrl);
        //        return Redirect(vnpayUrl);
        //    }

        //    TempData["ThongBao"] = "Phương thức thanh toán không hợp lệ.";
        //    return RedirectToAction("DanhSachHoaDon");
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoaDon(string id, string ngayThanhToan)
        {
            int idThanhToan;
            if (!int.TryParse(id, out idThanhToan))
            {
                TempData["ThongBao"] = "ID hóa đơn không hợp lệ.";
                return RedirectToAction("DanhSachHoaDon");
            }

            var thanhToan = db.ThanhToans.FirstOrDefault(t => t.IDThanhToan == idThanhToan);
            if (thanhToan == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn.";
                return RedirectToAction("DanhSachHoaDon");
            }

            var lopHoc = db.LopHocs.FirstOrDefault(l => l.IDLopHoc == thanhToan.IDLopHoc);
            if (lopHoc == null)
            {
                TempData["ThongBao"] = "Không tìm thấy lớp học cho hóa đơn này.";
                return RedirectToAction("DanhSachHoaDon");
            }

            // Parse ngày thanh toán từ chuỗi
            DateTime parsedNgayThanhToan;
            if (!DateTime.TryParse(ngayThanhToan, out parsedNgayThanhToan))
            {
                parsedNgayThanhToan = DateTime.Now;
            }

            // Cập nhật thông tin thanh toán
            thanhToan.PhuongThucTT = "VNPAY";
            thanhToan.NgayThanhToan = parsedNgayThanhToan;
            thanhToan.TrangThai = "Chờ xử lý";

            db.SaveChanges();

            // Tạo URL thanh toán VNPAY và chuyển hướng
            string returnUrl = Url.Action("VnpReturn", "ThanhToan", null, Request.Url.Scheme);
            return ThanhToanVNPay(thanhToan.IDThanhToan);
        }

        // Hàm tạo URL thanh toán VNPAY
        public ActionResult ThanhToanVNPay(int id)
        {
            var hoaDon = db.ThanhToans.FirstOrDefault(h => h.IDThanhToan == id && h.TrangThai == "Chưa thanh toán");
            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn.";
                return RedirectToAction("DanhSachHoaDon");
            }

            VnPayLibrary vnpay = new VnPayLibrary();
            string vnp_Returnurl = System.Configuration.ConfigurationManager.AppSettings["Vnp_ReturnUrl"];
            string vnp_Url = System.Configuration.ConfigurationManager.AppSettings["Vnp_Url"];
            string vnp_TmnCode = System.Configuration.ConfigurationManager.AppSettings["Vnp_TmnCode"];
            string vnp_HashSecret = System.Configuration.ConfigurationManager.AppSettings["Vnp_HashSecret"];

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)(hoaDon.SoTien.GetValueOrDefault() * 100)).ToString()); // x100 vì VNPay yêu cầu đơn vị là VND * 100
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan khoa hoc {hoaDon.IDKhoaHoc}");
            vnpay.AddRequestData("vnp_OrderType", "education");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", hoaDon.IDThanhToan.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Redirect(paymentUrl);
        }

        // Hàm tạo chữ ký HMAC SHA512
        public static string HashHmacSHA512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
            }
        }

        // Xử lý callback trả về từ VNPAY
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

            string vnp_HashSecret = System.Configuration.ConfigurationManager.AppSettings["Vnp_HashSecret"];
            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
            bool isValid = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

            int idThanhToan = int.Parse(Request.QueryString["vnp_TxnRef"]);
            var hoaDon = db.ThanhToans.FirstOrDefault(h => h.IDThanhToan == idThanhToan);

            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn.";
                return RedirectToAction("DanhSachHoaDon");
            }

            if (isValid)
            {
                string responseCode = Request.QueryString["vnp_ResponseCode"];
                string transactionNo = Request.QueryString["vnp_TransactionNo"];
                string amount = Request.QueryString["vnp_Amount"];

                // Lưu giao dịch
                db.GiaoDichVNPAYs.Add(new GiaoDichVNPAY
                {
                    IDThanhToan = hoaDon.IDThanhToan,
                    MaGiaoDich = transactionNo,
                    NgayGiaoDich = DateTime.Now,
                    SoTien = hoaDon.SoTien,
                    TrangThai = responseCode == "00" ? "Thành công" : "Thất bại",
                    NoiDung = $"Thanh toán cho hóa đơn #{hoaDon.IDThanhToan}",
                    PhanHoiVNPAY = responseCode
                });

                if (responseCode == "00")
                {
                    hoaDon.TrangThai = "Chờ xử lý";
                }

                db.SaveChanges();

                TempData["ThongBao"] = responseCode == "00" ? "Thanh toán thành công." : "Thanh toán thất bại.";
            }
            else
            {
                TempData["ThongBao"] = "Xác thực không hợp lệ.";
            }

            return RedirectToAction("DanhSachHoaDon");
        }
    }
}
