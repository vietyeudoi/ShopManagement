using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Models;

namespace Sieu_Thi_Mini.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProfileController : BaseCustomerController
    {
        private readonly ShopManagementContext _context;

        public ProfileController(ShopManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            // ✅ Include Addresses để load collection
            var customer = _context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
                return RedirectToAction("Login", "Account");

            return View(customer);
        }

        [HttpPost]
        public IActionResult Index(Sieu_Thi_Mini.Models.Customer model, List<string> addressList)
        {
            var customer = _context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefault(c => c.CustomerId == model.CustomerId);

            if (customer == null)
                return NotFound();

            // ✅ Cập nhật thông tin cơ bản
            customer.FullName = model.FullName;
            customer.Phone = model.Phone;

            // ✅ Xóa tất cả địa chỉ cũ
            var oldAddresses = customer.Addresses.ToList();
            _context.Addresses.RemoveRange(oldAddresses);

            // ✅ Thêm địa chỉ mới từ form
            if (addressList != null && addressList.Any())
            {
                foreach (var addr in addressList.Where(a => !string.IsNullOrWhiteSpace(a)))
                {
                    customer.Addresses.Add(new Address
                    {
                        CustomerId = customer.CustomerId,
                        Address1 = addr.Trim()
                    });
                }
            }

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddAddress(string newAddress)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return Json(new { success = false, message = "Chưa đăng nhập" });

            if (string.IsNullOrWhiteSpace(newAddress))
                return Json(new { success = false, message = "Địa chỉ không được để trống" });

            var address = new Address
            {
                CustomerId = customerId.Value,
                Address1 = newAddress.Trim()
            };

            _context.Addresses.Add(address);
            _context.SaveChanges();

            return Json(new { success = true, message = "Thêm địa chỉ thành công" });
        }

        [HttpPost]
        public IActionResult DeleteAddress(int addressId)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return Json(new { success = false, message = "Chưa đăng nhập" });

            var address = _context.Addresses
                .FirstOrDefault(a => a.AddressId == addressId && a.CustomerId == customerId);

            if (address == null)
                return Json(new { success = false, message = "Không tìm thấy địa chỉ" });

            _context.Addresses.Remove(address);
            _context.SaveChanges();

            return Json(new { success = true, message = "Xóa địa chỉ thành công" });
        }

        [HttpPost]
        public IActionResult UpdateAddress(int addressId, string newAddress)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return Json(new { success = false, message = "Chưa đăng nhập" });

            if (string.IsNullOrWhiteSpace(newAddress))
                return Json(new { success = false, message = "Địa chỉ không được để trống" });

            var address = _context.Addresses
                .FirstOrDefault(a => a.AddressId == addressId && a.CustomerId == customerId);

            if (address == null)
                return Json(new { success = false, message = "Không tìm thấy địa chỉ" });

            address.Address1 = newAddress.Trim();
            _context.SaveChanges();

            return Json(new { success = true, message = "Cập nhật địa chỉ thành công" });
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(
            string CurrentPassword,
            string NewPassword,
            string ConfirmPassword)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var customer = _context.Customers.Find(customerId);
            if (customer == null)
                return RedirectToAction("Login", "Account");

            // ❌ Kiểm tra mật khẩu hiện tại
            if (!BCrypt.Net.BCrypt.Verify(CurrentPassword, customer.Password))
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng";
                return View();
            }

            // ❌ Kiểm tra mật khẩu mới không khớp
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "Xác nhận mật khẩu không khớp";
                return View();
            }

            // ❌ Kiểm tra mật khẩu mới trùng mật khẩu cũ
            if (BCrypt.Net.BCrypt.Verify(NewPassword, customer.Password))
            {
                ViewBag.Error = "Mật khẩu mới phải khác mật khẩu cũ";
                return View();
            }

            // ✅ Kiểm tra độ dài mật khẩu
            if (NewPassword.Length < 6)
            {
                ViewBag.Error = "Mật khẩu mới phải có ít nhất 6 ký tự";
                return View();
            }

            // ✅ Hash mật khẩu mới và lưu
            customer.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            _context.SaveChanges();

            ViewBag.Success = "Đổi mật khẩu thành công";
            return View();
        }
    }
}