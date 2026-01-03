using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Models;
using System.Diagnostics;

namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ShopManagementContext _context;
        public DashboardController(ShopManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.TotalUser = _context.Users.Count();

            ViewBag.TotalCustomer = _context.Customers.Count();

            ViewBag.TotalProduct = _context.Products.Count();

            ViewBag.TotalOrder = _context.Orders.Count();

            ViewBag.TodayRevenue = _context.Orders
                .Where(o => o.OrderDate >= DateTime.Today
                         && o.OrderDate < DateTime.Today.AddDays(1))
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            ViewBag.LatestOrders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();

            return View();
        }

    }
}
