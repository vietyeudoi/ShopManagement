using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Filters;
using Sieu_Thi_Mini.Models;
using System.Data;
using System.Diagnostics;

namespace Sieu_Thi_Mini.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopManagementContext _context;
        public HomeController (ShopManagementContext context)
        {
            _context = context;
            
        }
        //[RoleAuthorize("Customer")]
        
        public IActionResult Index()
        {
            var product = _context.Products.ToList();
            if (HttpContext.Session.GetString("Role") == "Customer")
            {
                ViewData["Layout"] = "_LayoutCustomer";
            }      
            return View(product);
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
            if (HttpContext.Session.GetString("Role") == "Customer")
            {
                ViewData["Layout"] = "_LayoutCustomer";
            }
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
