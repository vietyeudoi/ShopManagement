using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Helpers;
using Sieu_Thi_Mini.Models;
using System.Collections.Generic;
using System.Linq;
using static Sieu_Thi_Mini.Models.Order;
using static Sieu_Thi_Mini.Models.Payment;

namespace Sieu_Thi_Mini.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : BaseCustomerController
    {
        private readonly ShopManagementContext _context;
        private const string CART_KEY = "CART";

        public OrderController(ShopManagementContext context)
        {
            _context = context;
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CART_KEY);
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            // ✅ Include Addresses để load collection
            var customer = _context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Customer = customer;

            return View(cart);
        }

        [HttpPost]
        [Area("Customer")]
        public IActionResult Checkout(OrderStatus orderStatus,
                              PaymentMethodEnum paymentMethod, string ShippingAddress)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CART_KEY);
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            // ✅ Include Addresses để load collection
            var customer = _context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
                return RedirectToAction("Login", "Account");

            // ✅ Lấy địa chỉ đầu tiên từ collection Addresses
            var firstAddress = customer.Addresses.FirstOrDefault();
            string addressText = firstAddress?.Address1 ?? "";

            // ❌ CHẶN nếu thiếu thông tin
            if (string.IsNullOrEmpty(customer.FullName)
                || string.IsNullOrEmpty(customer.Phone)
                || string.IsNullOrEmpty(addressText))
            {
                return Redirect("/Customer/Profile");
            }

            // 1️⃣ TẠO ORDER
            var order = new Order
            {
                CustomerId = customer.CustomerId,
                UserId = 1,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(c => c.Total),
                Status = orderStatus,
                Address = ShippingAddress // ✅ Dùng địa chỉ từ form
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            // 2️⃣ PAYMENT
            _context.Payments.Add(new Payment
            {
                OrderId = order.OrderId,
                PaymentMethod = paymentMethod,
                PaymentStatus = PaymentStatusEnum.Pending,
                PaymentDate = DateTime.Now
            });

            // 3️⃣ ORDER DETAIL
            foreach (var item in cart)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                });
            }

            _context.SaveChanges();

            HttpContext.Session.Remove(CART_KEY);

            return RedirectToAction("Success", new { id = order.OrderId });
        }


        // GET: /Customer/Order/Success?id=xxx
        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        public IActionResult History()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        public IActionResult Detail(int id)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Addresses) // ✅ Load Addresses của Customer
                .Include(o => o.Payment)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            var details = _context.OrderDetails
                .Where(d => d.OrderId == id)
                .Include(d => d.Product)
                .ToList();

            ViewBag.Order = order;

            return View(details);
        }
    }
}