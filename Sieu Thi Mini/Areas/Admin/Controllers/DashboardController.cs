using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Models;

namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ShopManagementContext _context;
        public DashboardController(ShopManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalUsers = _context.Users.Count();

            ViewBag.TotalProducts = _context.Products.Count();

            ViewBag.TotalRevenue = _context.Orders.Sum(o => o.TotalAmount);

            ViewBag.TotalOrders = _context.Orders.Count();

            var recentOrders = _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();

            return View(recentOrders);
        }
    }
}
