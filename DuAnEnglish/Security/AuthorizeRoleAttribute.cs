using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DuAnEnglish.Security
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedRoles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            this.allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session == null || httpContext.Session["User"] == null || httpContext.Session["Role"] == null)
            {
                return false;
            }

            string userRole = httpContext.Session["Role"].ToString().ToLower();
            if (allowedRoles == null || allowedRoles.Length == 0)
            {
                return true;
            }

            foreach (var role in allowedRoles)
            {
                if (string.Equals(userRole, role.ToLower(), System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session == null || filterContext.HttpContext.Session["User"] == null)
            {
                filterContext.Controller.TempData["ThongBaoDangNhap"] = "Vui lòng đăng nhập để tiếp tục.";
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "controller", "DangNhap" },
                        { "action", "DangNhap" }
                    });
            }
            else
            {
                filterContext.Controller.TempData["ThongBao"] = "Bạn không có quyền truy cập vào chức năng này!";
                string role = filterContext.HttpContext.Session["Role"] != null ? filterContext.HttpContext.Session["Role"].ToString().ToLower() : "";
                if (role == "admin")
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                            { "controller", "HomeAdmin" },
                            { "action", "Index" }
                        });
                }
                else if (role == "giangvien")
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                            { "controller", "HomeGiangVien" },
                            { "action", "Index" }
                        });
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                            { "controller", "HomeHocVien" },
                            { "action", "Index" }
                        });
                }
            }
        }
    }
}
