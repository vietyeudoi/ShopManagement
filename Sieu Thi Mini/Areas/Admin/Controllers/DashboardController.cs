using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Models;

namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    [Route("admin/")]
    public class DashboardController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

    }
}
