using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuAnEnglish.Controllers
{
    public class HomeAdminController : Controller
    {
        // GET: HomeAdmin
        public ActionResult Index()
        {
            //// Kiểm tra xem session có lưu đúng giá trị role không
            //if (Session["User"] != null)
            //{
            //    System.Diagnostics.Debug.WriteLine("User in session: " + Session["User"]);
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("No user found in session.");
            //}

            //if (Session["Role"] != null)
            //{
            //    System.Diagnostics.Debug.WriteLine("Role in session: " + Session["Role"]);
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("No role found in session.");
            //}

            // Còn lại phần xử lý logic của HomeAdmin nếu cần
            return View();
        }
    }
}