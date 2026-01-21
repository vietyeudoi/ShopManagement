using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

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
            var session = context.HttpContext.Session;
            var role = session.GetString("Role");

            if (string.IsNullOrEmpty(role))
            {
                var request = context.HttpContext.Request;
                var returnUrl = request.Path + request.QueryString;

                session.SetString("ReturnUrl", returnUrl);

                context.Result = new RedirectToActionResult(
                    "Login",
                    "Account",
                    new { area = "" }
                );
                return;
            }

            if (role != _role)
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Account",
                    new { area = "" }
                );
            }
        }
    }
}
