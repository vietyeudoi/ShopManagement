using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Models;
using System.Diagnostics;

namespace Sieu_Thi_Mini.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/category")]
    public class CategoryController : BaseAdminController
    {
        private readonly ShopManagementContext _context;

        public CategoryController(ShopManagementContext context)
        {
            _context = context;
        }

        // /admin/category
        [Route("")]
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // /admin/category/create
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                model.IsActive = true;
                _context.Categories.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // /admin/category/edit/5
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [Route("edit/{id}")]
        public IActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Ẩn danh mục (không xóa cứng)
        [Route("toggle/{id}")]
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
        [HttpGet("delete/{id}")]
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
        [HttpPost("delete/{id}")]
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
