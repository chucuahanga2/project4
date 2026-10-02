using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections.Generic;
using DuAnEnglish.Models;
using System.Linq;




namespace DuAnEnglish.Controllers
{
    public class TuVanController : Controller
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private trungtamtienganhEntities db = new trungtamtienganhEntities();



        // GET: TuVan
        public ActionResult TuVan()
        {
            // Lấy lịch sử chat từ Session nếu có
            var chatHistory = Session["ChatHistory"] as List<(string CauHoi, string TraLoi)>;
            if (chatHistory == null) chatHistory = new List<(string, string)>();

            ViewBag.ChatHistory = chatHistory;
            return View();
        }

        // Xử lý form gửi câu hỏi
        [HttpPost]
        public async Task<ActionResult> TuVan(string cauHoi)
        {
            var chatHistory = Session["ChatHistory"] as List<(string CauHoi, string TraLoi)>;
            if (chatHistory == null) chatHistory = new List<(string, string)>();

            string cauTraLoi = "";
            // Chuẩn hóa câu hỏi người dùng
            string cauHoiLower = cauHoi.ToLower();

            // Tìm các câu trong CSDL mà từ khóa khớp với câu hỏi người dùng
            var ketQuaPhuHop = db.ChatBotNoiDungs
                .Where(nd => !string.IsNullOrEmpty(nd.TuKhoa))
                .AsEnumerable()
                .Where(nd =>
                {
                    var tuKhoaArray = nd.TuKhoa.ToLower().Split(','); // cho phép nhiều từ khóa cách nhau bằng dấu phẩy
        return tuKhoaArray.Any(kw => cauHoiLower.Contains(kw.Trim()));
                })
                .OrderByDescending(nd => nd.TuKhoa.Length) // Ưu tiên từ khóa dài (cụ thể hơn)
                .FirstOrDefault();

            if (ketQuaPhuHop != null)
            {
                cauTraLoi = ketQuaPhuHop.CauTraLoi;
            }
            else
            {
                // Gọi Gemini nếu không tìm thấy câu trả lời
                string apiKey = WebConfigurationManager.AppSettings["GeminiApiKey"];
                cauTraLoi = await GoiGemini(cauHoi, apiKey);
            }


            // Lưu lịch sử chat
            chatHistory.Add((cauHoi, cauTraLoi));
            Session["ChatHistory"] = chatHistory;

            ViewBag.ChatHistory = chatHistory;
            return View();
        }


        // API Post JSON (giữ nguyên)
        [HttpPost]
        [Route("api/tuvan")]
        public async Task<ActionResult> ChatbotApi()
        {
            using (var reader = new StreamReader(Request.InputStream))
            {
                var bodyText = reader.ReadToEnd();

                System.Diagnostics.Debug.WriteLine("BODY RECEIVED: " + bodyText);

                var model = JsonConvert.DeserializeObject<ChatRequestModel>(bodyText);

                if (model == null || string.IsNullOrWhiteSpace(model.cauHoi))
                {
                    return Json(new { reply = "Câu hỏi không hợp lệ." });
                }

                string apiKey = WebConfigurationManager.AppSettings["GeminiApiKey"];
                string responseText = await GoiGemini(model.cauHoi, apiKey);
                return Json(new { reply = responseText });
            }
        }

        private async Task<string> GoiGemini(string inputText, string apiKey)
        {
            try
            {
                string url = string.Format("https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={0}", apiKey);

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new {
                                    text =
                                    "Bạn là chatbot tư vấn cho website khóa học trực tuyến. Chỉ trả lời về khóa học, học phí, bài giảng, giảng viên, ưu đãi. " +
                                    "Nếu được hỏi ngoài phạm vi, hãy nói 'Tôi chỉ hỗ trợ thông tin về các khóa học trên hệ thống.'\n\n" +
                                    "Câu hỏi: " + inputText
                                }
                            }
                        }
                    }
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, jsonContent);
                if (!response.IsSuccessStatusCode)
                {
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    return string.Format("Lỗi gọi Gemini API: {0}", errorDetails);
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseJson);

                try
                {
                    return result.candidates[0].content.parts[0].text.ToString();
                }
                catch
                {
                    return "Không có phản hồi từ AI.";
                }
            }
            catch (Exception ex)
            {
                return string.Format("Lỗi trong quá trình gọi API: {0}", ex.Message);
            }
        }

        public class ChatRequestModel
        {
            public string cauHoi { get; set; }
        }
    }
}