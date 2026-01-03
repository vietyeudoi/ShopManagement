using Microsoft.AspNetCore.Mvc;

namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
