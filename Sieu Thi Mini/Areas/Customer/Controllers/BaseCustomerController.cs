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

            ViewData["Layout"] = role switch
            {
                "Customer" => "_LayoutCustomer",
                "Admin" => "_LayoutCustomer",
                _ => "_Layout"
            };

            base.OnActionExecuting(context);
        }
    }
}
