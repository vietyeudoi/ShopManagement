using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Models;

namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportController : Controller
    {
        private readonly ShopManagementContext _context;
        public ReportController(ShopManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Tổng số khách hàng
            ViewBag.TotalUsers = _context.Users.Count();

            // Tổng số sản phẩm
            ViewBag.TotalProducts = _context.Products.Count();

            // Tổng doanh thu (Giả sử bảng Order có cột TotalAmount)
            ViewBag.TotalRevenue = _context.Orders.Sum(o => o.TotalAmount);

            // Số đơn hàng thành công
            ViewBag.TotalOrders = _context.Orders.Count();

            // Danh sách đơn hàng mới nhất (Top 5)
            var recentOrders = _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();

            return View(recentOrders);
        }
    }
}
