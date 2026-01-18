using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Models;
using System.Linq;

namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly ShopManagementContext _context;

        public CustomerController(ShopManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var customers = _context.Customers
                                    .OrderByDescending(c => c.CustomerId)
                                    .ToList();
            return View(customers);
        }

        public IActionResult Details(int id)
        {
            var customer = _context.Customers
                                   .FirstOrDefault(c => c.CustomerId == id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Sieu_Thi_Mini.Models.Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            bool emailExists = _context.Customers.Any(c => c.Email == customer.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return View(customer);
            }

            customer.IsActive = true;
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id,Sieu_Thi_Mini.Models.Customer customer)
        {
            if (id != customer.CustomerId) return NotFound();

            if (!ModelState.IsValid)
                return View(customer);

            bool emailExists = _context.Customers
                .Any(c => c.Email == customer.Email && c.CustomerId != customer.CustomerId);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return View(customer);
            }

            _context.Update(customer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            customer.IsActive = !customer.IsActive;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
