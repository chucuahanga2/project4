using System.Web.Mvc;
using DuAnEnglish.Security;

namespace DuAnEnglish.Controllers
{
    [AuthorizeRole("hocvien")]
    public class HomeHocVienController : Controller
    {
        // GET: HomeHocVien
        public ActionResult Index()
        {
            // Điều hướng trực tiếp về trang Khóa học của tôi
            return RedirectToAction("KhoaHocCuaToi", "HocTap");
        }
    }
}