using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class QuanLyChatBotController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: QuanLyChatBot
        public ActionResult QuanLyChatBot()
        {
            // Lấy toàn bộ nội dung chatbot từ cơ sở dữ liệu
            var ds = db.ChatBotNoiDungs.ToList();
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            return View(ds);
        }


        // GET: QuanLyChatBot/Create
        public ActionResult Them()
        {
            return View("Them");
        }

        // POST: QuanLyChatBot/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them([Bind(Include = "CauHoiMau,TuKhoa,CauTraLoi")] ChatBotNoiDung chatbot)
        {
            if (string.IsNullOrWhiteSpace(chatbot.CauHoiMau))
            {
                ViewBag.ThongBao = "Vui lòng không để trống trường câu hỏi";
                return View(chatbot);
            }
            else if (string.IsNullOrWhiteSpace(chatbot.TuKhoa))
            {
                ViewBag.ThongBao = "Vui lòng không để trống trường từ khóa";
                return View(chatbot);
            }
            else if (string.IsNullOrWhiteSpace(chatbot.CauTraLoi))
            {
                ViewBag.ThongBao = "Vui lòng không để trống trường câu trả lời";
                return View(chatbot);
            }

            db.ChatBotNoiDungs.Add(chatbot);
            db.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("QuanLyChatBot");
        }


        // GET: QuanLyChatBot/Sua/5
        public ActionResult ChiTiet(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var item = db.ChatBotNoiDungs.Find(id);
            if (item == null)
                return HttpNotFound();

            return View(item); // Hiển thị form sửa
        }

        // POST: QuanLyChatBot/Sua/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChiTiet([Bind(Include = "MaChat,CauHoiMau,TuKhoa,CauTraLoi")] ChatBotNoiDung chatbot)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chatbot).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Cập nhật nội dung thành công!";
                return RedirectToAction("QuanLyChatBot");
            }
            return View(chatbot);
        }

        // Xóa
        public ActionResult Xoa(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var item = db.ChatBotNoiDungs.Find(id);
            if (item == null)
                return HttpNotFound();

            db.ChatBotNoiDungs.Remove(item);
            db.SaveChanges();
            TempData["ThongBao"] = "Xoá nội dung thành công!";
            return RedirectToAction("QuanLyChatBot");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
