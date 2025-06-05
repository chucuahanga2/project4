using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class KhoaHocController : Controller
    {
        // Khởi tạo DbContext
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: KhoaHoc
        public ActionResult KhoaHoc(string danhmuc)
        {
            //Lấy toàn bộ danh sách khóa học từ cơ sở dữ liệu.
            var ds = db.KhoaHocs.ToList();

            //Kiểm tra xem biến 'danhmuc' có giá trị không và không phải là "all".
            //Nếu có giá trị và không phải là "all", tiến hành lọc danh sách khóa học theo 'DanhMuc'.
            if (!string.IsNullOrEmpty(danhmuc) && danhmuc != "all")
            {
                //Lọc danh sách khóa học theo danh mục ('DanhMuc') mà người dùng chọn.
                ds = ds.Where(k => k.DanhMuc == danhmuc).ToList();
            }

            // Trả lại danh sách khóa học (đã lọc hoặc tất cả) cho view để hiển thị.
            return View(ds);
        }

        // GET: Chi tiết khóa học
        public ActionResult ChiTietKhoaHoc(string id)
        {
            // Lấy thông tin khóa học theo ID
            var khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == id);

            // Nếu không tìm thấy khóa học, trả về lỗi 404
            if (khoaHoc == null)
            {
                return HttpNotFound();
            }

            // Trả thông tin khóa học cho view
            return View(khoaHoc);
        }

        // GET: /KhoaHoc/DangKyKhoaHoc
        public ActionResult DangKyKhoaHoc(string id, string lopId)
        {
            //Debug
            //System.Diagnostics.Debug.WriteLine(">>>> Gọi DangKyKhoaHoc");
            //System.Diagnostics.Debug.WriteLine(">>>> IDKhoaHoc: " + id);
            //System.Diagnostics.Debug.WriteLine(">>>> IDLopHoc: " + lopId);

            // Kiểm tra nếu chưa đăng nhập
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để đăng ký khóa học";
                return RedirectToAction("DangNhap", "DangNhap");
            }
            // Lấy TenDangNhap từ Session
            string tenDangNhap = Session["User"].ToString();

            // Lấy thông tin lớp học và khóa học dựa vào ID
            var lopHoc = db.LopHocs.FirstOrDefault(l => l.IDLopHoc == lopId);
            var khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == id);

            if (lopHoc == null || khoaHoc == null)
            {
                return HttpNotFound();
            }
            //Debug
            //System.Diagnostics.Debug.WriteLine(">>>> Slot HasValue: " + lopHoc.Slot.HasValue);  // Kiểm tra xem Slot có giá trị hay không
            //System.Diagnostics.Debug.WriteLine(">>>> Slot Value: " + lopHoc.Slot);  // Kiểm tra giá trị của Slot


            if (lopHoc.Slot.HasValue && lopHoc.Slot.Value == 0)
            {
                //System.Diagnostics.Debug.WriteLine(">>>> SLOT == 0, redirecting...");
                //System.Diagnostics.Debug.WriteLine(">>>> Redirect with IDKhoaHoc: " + id);
                TempData["ThongBao"] = "Lớp đã hết chỗ vui lòng chọn lớp khác";

                return RedirectToAction("DanhSachLopHoc", "LopHoc", new { id = id });

            }

            // Kiểm tra học viên đã đăng ký lớp này chưa
            var daDangKy = db.ThanhToans.Any(tt =>
                tt.TenDangNhap == tenDangNhap &&
                tt.IDKhoaHoc == id &&
                tt.IDLopHoc == lopId);

            if (daDangKy)
            {
                TempData["ThongBao"] = "Bạn đã đăng ký lớp này. Vui lòng thanh toán để hoàn tất!";
                return RedirectToAction("DanhSachLopHoc", "LopHoc", new { id = id });
            }
            // Lấy thông tin học viên thông qua TenDangNhap
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.IDTenDangNhap == tenDangNhap);
            if (hocVien == null)
            {
                return HttpNotFound(); // Hoặc xử lý khác nếu không có học viên
            }

            // Gán tên học viên vào ViewBag để hiển thị ở View
            ViewBag.TenHocVien = hocVien.TenHV;
            // Tạo model ThanhToan để truyền vào view
            var model = new ThanhToan
            {
                IDKhoaHoc = khoaHoc.IDKhoaHoc,
                IDLopHoc = lopHoc.IDLopHoc,
                KhoaHoc = khoaHoc,
                LopHoc = lopHoc,
                TenDangNhap = tenDangNhap

            };
            return View(model);
        }


        // POST: /KhoaHoc/DangKyKhoaHoc
        [HttpPost]
        public ActionResult DangKyKhoaHoc(ThanhToan model, string id, string lopId)
        {
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để đăng ký khóa học";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            string tenDangNhap = Session["User"].ToString();

            // Lấy thông tin lớp học và khóa học dựa trên ID
            var lopHoc = db.LopHocs.FirstOrDefault(l => l.IDLopHoc == model.IDLopHoc);
            var khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == model.IDKhoaHoc);

            if (lopHoc == null || khoaHoc == null)
            {
                return HttpNotFound();
            }

            // Tạo mới bản ghi ThanhToan
            ThanhToan thanhToan = new ThanhToan
            {
                IDLopHoc = model.IDLopHoc,
                IDKhoaHoc = model.IDKhoaHoc,
                TenDangNhap = tenDangNhap,
                SoTien = khoaHoc.HocPhi ?? 0m,
                PhuongThucTT = null,        // Chưa chọn phương thức thanh toán
                NgayThanhToan = null,       // Chưa thanh toán
                TrangThai = "Chưa thanh toán"
            };

            db.ThanhToans.Add(thanhToan);
            db.SaveChanges();
            
            TempData["ThongBao"] = "Đăng ký thành công! Vui lòng thanh toán để hoàn tất.";
            return RedirectToAction("DanhSachHoaDon", "ThanhToan");
        }
        //Code debug mau:
        // POST: /KhoaHoc/DangKyKhoaHoc
        //[HttpPost]
        //public ActionResult DangKyKhoaHoc(ThanhToan model, string id, string lopId)
        //{
        //    // Debug xem có giá trị trong model không
        //    //System.Diagnostics.Debug.WriteLine(">>> Model tại POST: " + model?.ToString());
        //    // Kiểm tra xem các giá trị có khớp không
        //    //System.Diagnostics.Debug.WriteLine(">>> Model IDKhoaHoc: " + model.IDKhoaHoc);
        //    //System.Diagnostics.Debug.WriteLine(">>> Model IDLopHoc: " + model.IDLopHoc);
        //    if (Session["User"] == null)
        //    {
        //        TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để đăng ký khóa học";
        //        return RedirectToAction("DangNhap", "DangNhap");
        //    }

        //    string tenDangNhap = Session["User"].ToString();

        //    // Debug: Kiểm tra giá trị TenDangNhap
        //    //System.Diagnostics.Debug.WriteLine(">>> TenDangNhap từ Session: " + tenDangNhap);

        //    // Debug: Kiểm tra giá trị IDKhoaHoc khi vào POST
        //    //System.Diagnostics.Debug.WriteLine(">>> Trước khi đăng ký, IDKhoaHoc: " + model.IDKhoaHoc);

        //    // Lấy thông tin lớp học và khóa học dựa trên ID
        //    var lopHoc = db.LopHocs.FirstOrDefault(l => l.IDLopHoc == model.IDLopHoc);
        //    var khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == model.IDKhoaHoc);

        //    // Debug: Kiểm tra thông tin lớp học và khóa học
        //    //if (lopHoc == null)
        //    //{
        //    //    System.Diagnostics.Debug.WriteLine(">>> Lớp học không tìm thấy, IDLopHoc: " + model.IDLopHoc);
        //    //}
        //    //if (khoaHoc == null)
        //    //{
        //    //    System.Diagnostics.Debug.WriteLine(">>> Khóa học không tìm thấy, IDKhoaHoc: " + model.IDKhoaHoc);
        //    //}

        //    if (lopHoc == null || khoaHoc == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    // Kiểm tra học viên đã đăng ký chưa
        //    //var daDangKy = db.ThanhToans.Any(tt =>
        //    //    tt.TenDangNhap == tenDangNhap &&
        //    //    tt.IDKhoaHoc == model.IDKhoaHoc &&
        //    //    tt.IDLopHoc == model.IDLopHoc);

        //    // Debug: Kiểm tra kết quả đăng ký
        //    //System.Diagnostics.Debug.WriteLine(">>> Học viên đã đăng ký chưa: " + daDangKy);

        //    //if (daDangKy)
        //    //{               
        //    //    //ViewBag.ThongBao = "Bạn đã đăng ký lớp học này. Vui lòng thanh toán để hoàn tất!";
        //    //    //return RedirectToAction("DanhSachHoaDon", "ThanhToan");
        //    //}

        //    // Tạo mới bản ghi ThanhToan
        //    ThanhToan thanhToan = new ThanhToan
        //    {
        //        IDLopHoc = model.IDLopHoc,
        //        IDKhoaHoc = model.IDKhoaHoc,
        //        TenDangNhap = tenDangNhap,
        //        SoTien = khoaHoc.HocPhi ?? 0m,
        //        PhuongThucTT = null,        // Chưa chọn phương thức thanh toán
        //        NgayThanhToan = null,       // Chưa thanh toán
        //        TrangThai = "Chưa thanh toán"
        //    };

        //    // Debug: Kiểm tra dữ liệu thanh toán mới
        //    //System.Diagnostics.Debug.WriteLine(">>> Tạo mới thanh toán: " +
        //    //    "IDLopHoc: " + thanhToan.IDLopHoc + ", " +
        //    //    "IDKhoaHoc: " + thanhToan.IDKhoaHoc + ", " +
        //    //    "TenDangNhap: " + thanhToan.TenDangNhap + ", " +
        //    //    "SoTien: " + thanhToan.SoTien);

        //    db.ThanhToans.Add(thanhToan);
        //    db.SaveChanges();

        //    // Sau khi tạo thanh toán thành công, lấy lại tên học viên
        //    //var hocVien = db.HocViens.FirstOrDefault(hv => hv.IDTenDangNhap == tenDangNhap);

        //    // Debug: Kiểm tra tên học viên
        //    //if (hocVien != null)
        //    //{
        //    //    System.Diagnostics.Debug.WriteLine(">>> Tên học viên: " + hocVien.TenHV);
        //    //    ViewBag.TenHocVien = hocVien.TenHV;
        //    //}
        //    //else
        //    //{
        //    //    System.Diagnostics.Debug.WriteLine(">>> Không tìm thấy học viên với TenDangNhap: " + tenDangNhap);
        //    //}

        //    //ViewBag.ThongBao = "Đăng ký thành công! Vui lòng thanh toán để hoàn tất.";

        //    // Lấy lại thông tin lớp và khoá để bind vào View
        //    //model.KhoaHoc = khoaHoc;
        //    //model.LopHoc = lopHoc;

        //    // Debug: Kiểm tra lại thông tin lớp và khóa học
        //    //System.Diagnostics.Debug.WriteLine(">>> Lớp học: " + lopHoc.TenLop);
        //    //System.Diagnostics.Debug.WriteLine(">>> Khóa học: " + khoaHoc.TenKhoaHoc);
        //    //System.Diagnostics.Debug.WriteLine(">>> Giá khóa học: " + khoaHoc.HocPhi);
        //    TempData["ThongBao"] = "Đăng ký thành công! Vui lòng thanh toán để hoàn tất.";
        //    return RedirectToAction("DanhSachHoaDon", "ThanhToan");
        //    //return View(model);
        //}

    }
}