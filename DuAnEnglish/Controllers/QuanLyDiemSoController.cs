using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DuAnEnglish.Models;
using DuAnEnglish.ViewModels;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("giangvien", "admin")]
    public class QuanLyDiemSoController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyDiemSo
        public ActionResult QuanLyDiemSo(string idLop)
        {
            // Lấy tên đăng nhập từ session
            string tenDangNhap = Session["User"] != null ? Session["User"].ToString() : null;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để đăng ký khóa học";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            // Tìm giảng viên theo tên đăng nhập
            var giangVien = db.GiangViens.FirstOrDefault(gv => gv.IDTenDangNhap == tenDangNhap);
            if (giangVien == null)
            {
                TempData["ThongBao"] = "Không tìm thấy thông tin giảng viên.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách lớp giảng viên đang quản lý
            var danhSachLop = db.LopHocs
                .Where(l => l.IDGiangVien == giangVien.IDGiangVien)
                .ToList();

            // Lọc lớp theo từ khóa nếu có
            if (!string.IsNullOrEmpty(idLop))
            {
                idLop = idLop.Trim().ToLower();
                danhSachLop = danhSachLop
                    .Where(l => l.IDLopHoc.ToLower().Contains(idLop))
                    .ToList();
            }

            // Lấy danh sách ID lớp
            var lopIDs = danhSachLop.Select(l => l.IDLopHoc.Trim()).ToList();

            // Gửi tên lớp đã tìm đến view để hiển thị lại
            ViewBag.TuKhoa = idLop;

            var diemTongHop = new List<DiemViewModel>();

            // Điểm IELTS
            var diemIelts = db.DiemIELTS
                .Where(d => lopIDs.Contains(d.IDLopHoc.Trim()))
                .ToList();

            foreach (var item in diemIelts)
            {
                var lopHoc = db.LopHocs.FirstOrDefault(l => l.IDLopHoc.Trim() == item.IDLopHoc.Trim());
                var khoaHoc = db.KhoaHocs.FirstOrDefault(kh => kh.IDKhoaHoc == lopHoc.IDKhoaHoc);

                diemTongHop.Add(new DiemViewModel
                {
                    IDHocVien = item.IDHocVien,
                    IDLopHoc = item.IDLopHoc,
                    DanhMuc = (khoaHoc != null && khoaHoc.DanhMuc != null) ? khoaHoc.DanhMuc.Trim().ToLower() : null,
                    DiemNgheIELTS = item.DiemNghe,
                    DiemNoiIELTS = item.DiemNoi,
                    DiemDocIELTS = item.DiemDoc,
                    DiemVietIELTS = item.DiemViet,
                    TongDiemIELTS = item.TongDiem
                });
            }

            // Điểm TOEIC
            var diemToeic = db.DiemTOEICs
                .Where(d => lopIDs.Contains(d.IDLopHoc.Trim()))
                .ToList();
            foreach (var item in diemToeic)
            {
                var lopHoc = db.LopHocs.FirstOrDefault(l => l.IDLopHoc.Trim() == item.IDLopHoc.Trim());
                var khoaHoc = db.KhoaHocs.FirstOrDefault(kh => kh.IDKhoaHoc == lopHoc.IDKhoaHoc);

                diemTongHop.Add(new DiemViewModel
                {
                    IDHocVien = item.IDHocVien,
                    IDLopHoc = item.IDLopHoc,
                    DanhMuc = (khoaHoc != null && khoaHoc.DanhMuc != null) ? khoaHoc.DanhMuc.Trim().ToLower() : null,
                    DiemNgheTOEIC = item.DiemNghe,
                    DiemNoiTOEIC = item.DiemNoi,
                    DiemDocTOEIC = item.DiemDoc,
                    DiemVietTOEIC = item.DiemViet,
                    TongDiemTOEIC = item.TongDiem
                });
            }


            return View(diemTongHop);
        }
        // hiển thị gợi ý lớp
        public JsonResult GetLopHocAutocomplete(string term)
        {
            string tenDangNhap = Session["User"] != null ? Session["User"].ToString() : null;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }

            var giangVien = db.GiangViens.FirstOrDefault(gv => gv.IDTenDangNhap == tenDangNhap);
            if (giangVien == null)
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }

            var lopHocList = db.LopHocs
                .Where(l => l.IDGiangVien == giangVien.IDGiangVien && l.IDLopHoc.Contains(term))
                .Select(l => l.IDLopHoc)
                .Distinct()
                .ToList();

            return Json(lopHocList, JsonRequestBehavior.AllowGet);
        }



        // View nhập điểm IELTS
        public ActionResult NhapDiemIeltsView(int idHocVien)
        {
            // Tìm điểm IELTS theo idHocVien
            var diemIelts = db.DiemIELTS.FirstOrDefault(d => d.IDHocVien == idHocVien);

            // Nếu không tìm thấy thì tạo mới object điểm rỗng để truyền xuống View
            if (diemIelts == null)
            {
                diemIelts = new DiemIELT
                {
                    IDHocVien = idHocVien,
                    DiemNghe = null,
                    DiemNoi = null,
                    DiemDoc = null,
                    DiemViet = null,
                    TongDiem = null,
                    IDLopHoc = "" // hoặc null, tùy cấu trúc model
                };
            }

            return View(diemIelts);
        }

        [HttpPost]
        public ActionResult LuuDiemIelts(DiemIELT model)
        {
            if (ModelState.IsValid)
            {
                var diemIelts = db.DiemIELTS.FirstOrDefault(d => d.IDHocVien == model.IDHocVien);
                if (diemIelts == null)
                {
                    // Tạo mới
                    db.DiemIELTS.Add(model);
                }
                else
                {
                    // Cập nhật
                    diemIelts.DiemNghe = model.DiemNghe;
                    diemIelts.DiemNoi = model.DiemNoi;
                    diemIelts.DiemDoc = model.DiemDoc;
                    diemIelts.DiemViet = model.DiemViet;
                    diemIelts.TongDiem = model.TongDiem;
                }

                db.SaveChanges();
                TempData["ThongBao"] = "Lưu điểm IELTS thành công!";
                // Bạn có thể redirect về danh sách hoặc trang khác
                return RedirectToAction("QuanLyDiemSo");
            }

            // Nếu model không hợp lệ, trả lại View với dữ liệu hiện tại
            return View("NhapDiemIeltsView", model);
        }


        // View nhập điểm TOEIC (tạo mới)
        public ActionResult NhapDiemToeicView(int idHocVien)
        {
            var diemToeic = db.DiemTOEICs.FirstOrDefault(d => d.IDHocVien == idHocVien);

            if (diemToeic == null)
            {
                diemToeic = new DiemTOEIC
                {
                    IDHocVien = idHocVien,
                    Part1 = null,
                    Part2 = null,
                    Part3 = null,
                    Part4 = null,
                    Part5 = null,
                    Part6 = null,
                    Part7 = null,
                    DiemNoi = null,
                    DiemViet = null,
                    DiemNghe = null,
                    DiemDoc = null,
                    TongDiem = null,
                    IDLopHoc = "" // bạn có thể set IDLopHoc nếu có dữ liệu
                };
            }

            return View(diemToeic);
        }
        [HttpPost]
        public ActionResult LuuDiemToeic(DiemTOEIC model)
        {
            if (ModelState.IsValid)
            {
                // Quy đổi điểm nghe = tổng câu đúng của Part1-4 * hệ số quy đổi (ví dụ 5 điểm / câu)
                int nghe = (model.Part1 ?? 0) + (model.Part2 ?? 0) + (model.Part3 ?? 0) + (model.Part4 ?? 0);
                int doc = (model.Part5 ?? 0) + (model.Part6 ?? 0) + (model.Part7 ?? 0);

                int diemNghe = nghe * 5; // ví dụ 1 câu đúng = 5 điểm
                int diemDoc = doc * 5;   // tương tự

                model.DiemNghe = diemNghe;
                model.DiemDoc = diemDoc;

                // Tổng điểm = điểm nghe + điểm đọc + điểm nói + điểm viết
                model.TongDiem = diemNghe + diemDoc + (model.DiemNoi ?? 0) + (model.DiemViet ?? 0);

                var diemToeic = db.DiemTOEICs.FirstOrDefault(d => d.IDHocVien == model.IDHocVien);
                if (diemToeic == null)
                {
                    db.DiemTOEICs.Add(model);
                }
                else
                {
                    diemToeic.Part1 = model.Part1;
                    diemToeic.Part2 = model.Part2;
                    diemToeic.Part3 = model.Part3;
                    diemToeic.Part4 = model.Part4;
                    diemToeic.Part5 = model.Part5;
                    diemToeic.Part6 = model.Part6;
                    diemToeic.Part7 = model.Part7;
                    diemToeic.DiemNoi = model.DiemNoi;
                    diemToeic.DiemViet = model.DiemViet;
                    diemToeic.DiemNghe = model.DiemNghe;
                    diemToeic.DiemDoc = model.DiemDoc;
                    diemToeic.TongDiem = model.TongDiem;
                    //diemToeic.IDLopHoc = model.IDLopHoc;
                }

                db.SaveChanges();
                TempData["ThongBao"] = "Lưu điểm TOEIC thành công!";
                return RedirectToAction("QuanLyDiemSo");
            }

            return View("NhapDiemToeicView", model);
        }


    }
}
