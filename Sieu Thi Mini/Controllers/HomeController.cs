using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Models;

namespace Sieu_Thi_Mini.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Contact()
        {
            return View(new ContactViewModel());
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.ThongBao = "Gui lien he thanh cong!";
            }

            return View(model);
        }
    }
}
