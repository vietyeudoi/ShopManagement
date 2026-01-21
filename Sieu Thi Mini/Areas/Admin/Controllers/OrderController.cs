using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Models;
using System;
using System.Data;
using static Sieu_Thi_Mini.Models.Order;
namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : BaseAdminController
    {
        private readonly ShopManagementContext _context;

        public OrderController(ShopManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var orders = _context.Orders.Include(o => o.User).Include(o => o.Customer).OrderByDescending(o => o.OrderDate).ToList();
            return View(orders);
        }



        public IActionResult Details(string id) 
        {

            if (!int.TryParse(id, out int orderId))
            {
                return BadRequest("Invalid order ID");
            }

            var order = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
                return NotFound();

            var payment = _context.Payments.FirstOrDefault(p => p.OrderId == orderId);
            ViewBag.Payment = payment;

            return View(order);
        }


        [HttpPost]
        public IActionResult ChangeStatus(int orderId, OrderStatus status)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null) return NotFound();

            order.Status = status;
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = orderId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePaymentStatus(
    int orderId,
    Payment.PaymentStatusEnum status)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound();

            if (order.Payment == null)
            {
                TempData["Error"] = "Đơn hàng chưa có thông tin thanh toán.";
                return RedirectToAction("Details", new { id = orderId });
            }


            order.Payment.PaymentStatus = status;

            if (status == Payment.PaymentStatusEnum.Paid)
            {
                order.Payment.PaymentDate = DateTime.Now;

                order.Status = Order.OrderStatus.Confirmed;
            }else if(status == Payment.PaymentStatusEnum.Failed)
            {
                order.Payment.PaymentDate = DateTime.Now;

                order.Status = Order.OrderStatus.Cancelled;
            }    

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật trạng thái thanh toán thành công.";
            return RedirectToAction("Details", new { id = orderId });
        }

    }
}
