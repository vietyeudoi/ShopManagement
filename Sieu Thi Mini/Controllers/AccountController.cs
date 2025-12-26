using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Models;
using Sieu_Thi_Mini.Helpers;

namespace Sieu_Thi_Mini.Controllers
{
    public class AccountController : Controller
    {
        private readonly ShopManagementContext _context;
        public AccountController (ShopManagementContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.IsActive == true);
            if (user != null && PasswordHelper.VerifyPassword(password, user.Password))
            {
                SignInUser(user);

                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                if (user.Role == "Staff")
                    return RedirectToAction("Index", "Home", new { area = "Staff" });
            }

            var customer = _context.Customers.FirstOrDefault(c => c.Email == email && c.IsActive == true );
            if (customer != null && PasswordHelper.VerifyPassword(password, customer.Password))
            {
                SignInCustomer(customer);

                return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
            }
            ViewBag.Error = "Email hoặc mật khẩu không đúng";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Register(string fullName, string email, string password)
        {
            // 1️⃣ Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            // 2️⃣ Kiểm tra email tồn tại (Users + Customers)
            bool emailExists =
                _context.Users.Any(u => u.Email == email) ||
                _context.Customers.Any(c => c.Email == email);

            if (emailExists)
            {
                ViewBag.Error = "Email đã được sử dụng";
                return View();
            }

            // 3️⃣ Kiểm tra password tối thiểu
            if (password.Length < 6)
            {
                ViewBag.Error = "Mật khẩu phải ít nhất 6 ký tự";
                return View();
            }

            // 4️⃣ Tạo Customer
            var customer = new Customer
            {
                FullName = fullName,
                Email = email,
                Password = PasswordHelper.HashPassword(password),
                IsActive = true
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            SignInCustomer(customer);

            return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Index()
        {
            return View();
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
        private void SignInUser(User user)
        {
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("FullName", user.FullName);
        }

        private void SignInCustomer(Customer customer)
        {
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
            HttpContext.Session.SetString("CustomerName", customer.FullName);
            HttpContext.Session.SetString("Role", "Customer");
        }

    }
}
