using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Filters;

namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RoleAuthorize("Admin")]
    public abstract class BaseAdminController : Controller
    {
    }
}
