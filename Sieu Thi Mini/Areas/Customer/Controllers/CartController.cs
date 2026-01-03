using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Models;
using Sieu_Thi_Mini.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Sieu_Thi_Mini.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private const string CART_KEY = "CART";

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") == "Customer")
            {
                ViewData["Layout"] = "_LayoutCustomer";
            }
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CART_KEY)
                       ?? new List<CartItem>();
            return View(cart);
        }
        private readonly ShopManagementContext _context;

        public CartController(ShopManagementContext context)
        {
            _context = context;
        }

        public IActionResult Add(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CART_KEY)
                       ?? new List<CartItem>();

            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
                return NotFound();

            var item = cart.FirstOrDefault(p => p.ProductId == id);
            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = 1,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                item.Quantity++;
            }

            HttpContext.Session.SetObject(CART_KEY, cart);
            return Redirect(Request.Headers["Referer"].ToString());
        }


        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CART_KEY);
            if (cart != null)
            {
                var item = cart.FirstOrDefault(p => p.ProductId == id);
                if (item != null)
                {
                    cart.Remove(item);
                }
                HttpContext.Session.SetObject(CART_KEY, cart);
            }
            return RedirectToAction("Index");
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
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
                return NotFound();

            var details = _context.OrderDetails
                .Where(d => d.OrderId == id)
                .Select(d => new
                {
                    ProductName = d.Product.ProductName,
                    d.Quantity,
                    d.UnitPrice
                })
                .ToList();

            ViewBag.Order = order;
            return View(details);
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int delta)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CART_KEY)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item == null)
                return RedirectToAction("Index");

            // Xóa sản phẩm
            if (delta <= -999)
            {
                cart.Remove(item);
            }
            else
            {
                item.Quantity += delta;

                // Không cho nhỏ hơn 1
                if (item.Quantity < 1)
                {
                    cart.Remove(item);
                }
            }

            HttpContext.Session.SetObject(CART_KEY, cart);
            return RedirectToAction("Index");
        }
    }
}