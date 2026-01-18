using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Filters;
using Sieu_Thi_Mini.Models;
using System.Data;
using System.Diagnostics;

namespace Sieu_Thi_Mini.Controllers
{
    public class HomeController : Controller
    {
        public override void OnActionExecuting(
            Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role == "Customer" || role == "Admin")
            {
                ViewData["Layout"] = "_LayoutCustomer";
            }
            else
            {
                ViewData["Layout"] = "_Layout";
            }

            base.OnActionExecuting(context);
        }
        private readonly ShopManagementContext _context;
        public HomeController (ShopManagementContext context)
        {
            _context = context;
            
        }
        //[RoleAuthorize("Customer")]

        public IActionResult Index(int? categoryId)
        {
            ViewBag.Categories = _context.Categories
                .Where(c => c.IsActive)
                .ToList();

            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.Category.IsActive)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
                ViewBag.CategoryId = categoryId;
            }

            return View(products.ToList());

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
        [Route("/Contact")]
        public IActionResult Contact()
        {
            return View(new ContactViewModel());
        }
        public IActionResult Search(string keyword, int? categoryId)
        {
            ViewBag.Categories = _context.Categories
                .Where(c => c.IsActive)
                .ToList();

            ViewBag.CategoryId = categoryId;
            ViewBag.Keyword = keyword;

            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.Category.IsActive)
                .AsQueryable();

            if (categoryId == null && !string.IsNullOrWhiteSpace(keyword))
            {
                products = products.Where(p => p.ProductName.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
                ViewBag.Keyword = null;
            }

            return View(products.ToList());

        }

        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p =>
                    p.ProductId == id &&
                    p.IsActive &&
                    p.Category.IsActive);

            if (product == null)
                return NotFound();

            var relatedProducts = _context.Products
                .Where(p =>
                    p.IsActive &&
                    p.Category.IsActive &&
                    p.CategoryId == product.CategoryId &&
                    p.ProductId != product.ProductId)
                .Take(4)
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
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
