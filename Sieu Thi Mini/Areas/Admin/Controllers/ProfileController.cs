using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Models;

namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfileController : BaseAdminController
    {
        private readonly ShopManagementContext _context;
        public ProfileController(ShopManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.Find(userId);
            return View(user);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Sieu_Thi_Mini.Models.User model)
        {
            var user = _context.Users.Find(model.UserId);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Phone = model.Phone;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangePassword(
            string CurrentPassword,
            string NewPassword,
            string ConfirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // ❌ sai mật khẩu hiện tại
            if (!BCrypt.Net.BCrypt.Verify(CurrentPassword, user.Password))
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
            if (BCrypt.Net.BCrypt.Verify(NewPassword, user.Password))
            {
                ViewBag.Error = "Mật khẩu mới phải khác mật khẩu cũ";
                return View();
            }

            // ✅ hash mật khẩu mới
            user.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            _context.SaveChanges();

            ViewBag.Success = "Đổi mật khẩu thành công";
            return View();
        }




    }
}
