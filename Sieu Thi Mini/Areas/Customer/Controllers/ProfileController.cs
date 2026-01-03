using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Models;

namespace Sieu_Thi_Mini.Areas.Customer.Controllers
{
    public class ProfileController : BaseCustomerController
    {
        private readonly ShopManagementContext _context;
        public ProfileController (ShopManagementContext context)
        {
            _context = context;
        }
        [Area("Customer")]
        public IActionResult Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            var cus = _context.Customers.Find(customerId);
            cus?.LoadAdress();
            return View(cus);
        }

        [HttpPost]
        public IActionResult Index(Sieu_Thi_Mini.Models.Customer model)
        {
            var customer = _context.Customers.Find(model.CustomerId);
            if (customer == null) return NotFound();

            customer.FullName = model.FullName;
            customer.Phone = model.Phone;

            customer.ListAdress = model.ListAdress ?? new();
            customer.JAdress();

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteAddress(int index)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null) return Json(new { success = false });

            var customer = _context.Customers.Find(customerId);
            if (customer == null) return Json(new { success = false });

            customer.LoadAdress();

            if (index < 0 || index >= customer.ListAdress.Count)
                return Json(new { success = false });

            customer.ListAdress.RemoveAt(index);
            customer.JAdress();

            _context.SaveChanges();

            return Json(new { success = true });
        }

        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ChangePassword(
            string CurrentPassword,
            string NewPassword,S
            string ConfirmPassword)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var customer = _context.Customers.Find(customerId);
            if (customer == null)
                return RedirectToAction("Login", "Account");

            // ❌ sai mật khẩu hiện tại
            if (!BCrypt.Net.BCrypt.Verify(CurrentPassword, customer.Password))
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng";
                return View();
            }

            // ❌ mật khẩu mới không khớp
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "Xác nhận mật khẩu không khớp";
                return View();
            }

            // ❌ mật khẩu mới trùng mật khẩu cũ
            if (BCrypt.Net.BCrypt.Verify(NewPassword, customer.Password))
            {
                ViewBag.Error = "Mật khẩu mới phải khác mật khẩu cũ";
                return View();
            }

            // ✅ hash mật khẩu mới
            customer.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            _context.SaveChanges();

            ViewBag.Success = "Đổi mật khẩu thành công";
            return View();
        }




    }
}
