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


    }
}
