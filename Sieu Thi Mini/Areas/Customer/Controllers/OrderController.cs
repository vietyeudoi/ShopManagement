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

        // GET: /Customer/Order/Checkout
        public IActionResult Checkout()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CART_KEY);
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            return View(cart);
        }

        // POST: /Customer/Order/Checkout
        [HttpPost]
        public IActionResult Checkout(Sieu_Thi_Mini.Models.Customer customer, OrderStatus OrderStatus, PaymentStatusEnum paymentStatus , PaymentMethodEnum paymentMethod)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CART_KEY);
            if (cart == null || !cart.Any()) return RedirectToAction("Index", "Cart");

            // 1. Lưu hoặc xử lý thông tin khách hàng
            customer.Password = "checkout_guest"; // Mật khẩu mặc định cho khách vãng lai
            customer.IsActive = true;
            _context.Customers.Add(customer);
            _context.SaveChanges();

            // 2. Tạo đơn hàng (Lưu Ghi chú vào Status hoặc xử lý riêng nếu bạn có cột Note)
            var order = new Order
            {
                CustomerId = customer.CustomerId,
                UserId = 1, // Tạm thời để mặc định, sau này thay bằng User.Identity
                TotalAmount = cart.Sum(c => c.Total),
                OrderDate = DateTime.Now,
                // Nếu bảng Order chưa có cột Ghi chú, tạm lưu vào Status hoặc bạn có thể bỏ qua
                Status = OrderStatus
            };

            _context.Orders.Add(order);
            _context.SaveChanges(); // Sau dòng này, order.OrderId sẽ được cập nhật từ DB

            // 3. Lưu Phương thức thanh toán vào bảng Payment
            var paymentEntry = new Payment
            {
                OrderId = order.OrderId, // Sử dụng ID vừa tạo
                PaymentMethod = paymentMethod, // Nhận giá trị "COD" hoặc "Bank" từ Form
                PaymentDate = DateTime.Now,
                PaymentStatus = paymentStatus
            };
            _context.Payments.Add(paymentEntry);

            // 4. Lưu chi tiết đơn hàng
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

            // 5. Xóa giỏ hàng sau khi đặt thành công
            HttpContext.Session.Remove(CART_KEY);

            // QUAN TRỌNG: Truyền order.OrderId vào tham số của trang Success
            return RedirectToAction("Success", new { id = order.OrderId });
        }

        // GET: /Customer/Order/Success?id=xxx
        public IActionResult Success(int id)
        {
            // Nhận ID từ URL và truyền ra View qua ViewBag
            ViewBag.OrderId = id;

            return View();
        }

        public IActionResult History()
        {
            // TẠM THỜI: lấy tất cả đơn (sau này gắn login)
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
                .Include(o => o.Customer) // Lấy thông tin khách hàng
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null) return NotFound();

            var details = _context.OrderDetails
                .Include(d => d.Product) // Lấy thông tin sản phẩm để có ảnh và tên
                .Where(d => d.OrderId == id)
                .ToList();

            ViewBag.Order = order;
            return View(details);
        }
    }
}