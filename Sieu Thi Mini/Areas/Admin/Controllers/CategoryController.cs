using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Models;
using System.Diagnostics;

namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : BaseAdminController
    {
        private readonly ShopManagementContext _context;

        public CategoryController(ShopManagementContext context)
        {
            _context = context;
        }

        // /admin/category
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // /admin/category/create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine("LỖI VALIDATION: " + error.ErrorMessage);
                }
                return View(model); 
            }

            try
            {
                _context.Categories.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi Database: " + ex.Message);
            }
            return View(model);
        }

        // /admin/category/edit/5
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(int id, Category model)
        {
            if (id != model.CategoryId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Categories.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Ẩn danh mục (không xóa cứng)
        public IActionResult Toggle(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.IsActive = !category.IsActive;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: admin/category/delete/5
        public IActionResult Delete(int id)
        {
            var category = _context.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.CategoryId == id);

            if (category == null) return NotFound();

            if (category.Products.Any())
            {
                TempData["Error"] = "Danh mục đang có sản phẩm, không thể xoá!";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // POST: admin/category/delete/5
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }

}
