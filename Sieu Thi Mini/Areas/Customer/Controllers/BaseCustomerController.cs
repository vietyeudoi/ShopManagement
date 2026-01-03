using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Filters;

namespace Sieu_Thi_Mini.Areas.Customer.Controllers
{
    [Area("Customer")]
    [RoleAuthorize("Customer")]
    public abstract class BaseCustomerController : Controller
    {
        public override void OnActionExecuting(
            Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role == "Customer")
            {
                ViewData["Layout"] = "_LayoutCustomer";
            }
            else if (role == "Admin")
            {
                ViewData["Layout"] = "_LayoutAdmin";
            }
            else
            {
                ViewData["Layout"] = "_Layout";
            }

            base.OnActionExecuting(context);
        }
    }
}
