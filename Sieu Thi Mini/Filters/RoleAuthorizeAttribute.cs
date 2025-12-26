using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sieu_Thi_Mini.Filters
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _role;

        public RoleAuthorizeAttribute(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var controller = context.RouteData.Values["controller"]?.ToString();

            // Bỏ qua AccountController
            if (controller == "Account")
                return;

            var session = context.HttpContext.Session;
            var role = session.GetString("Role");

            if (string.IsNullOrEmpty(role))
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Account",
                    new { area = "" }
                );
                return;
            }

            if (!role.Equals(_role, StringComparison.OrdinalIgnoreCase))
            {
                session.Clear();
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Account",
                    new { area = "" }
                );
            }
        }

    }
}
