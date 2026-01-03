using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sieu_Thi_Mini.Areas.Admin.Controllers;
using Sieu_Thi_Mini.Models;
using System;

[Area("Admin")]
[Route("admin/product")]
public class ProductController : BaseAdminController
{
    private readonly ShopManagementContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductController(ShopManagementContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        return View(products);
    }


    [HttpGet("create")]
    public IActionResult Create()
    {
        LoadCategories();
        return View(new Product());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product, IFormFile ImageFile)
    {
        if (!ModelState.IsValid)
        {
            LoadCategories(product.CategoryId);
            return View(product);
        }

        if (ImageFile != null && ImageFile.Length > 0)
        {
            string uploadFolder = Path.Combine(_env.WebRootPath, "upload");

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            string filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                ImageFile.CopyTo(stream);
            }

            product.ImageUrl = fileName; 
        }

        product.CreatedAt = DateTime.Now;
        product.IsActive = true;

        _context.Products.Add(product);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }


    [HttpGet("edit/{id}")]
    public IActionResult Edit(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();

        LoadCategories(product.CategoryId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("edit/{id}")]
    public IActionResult Edit(int id, Product model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            LoadCategories(model.CategoryId);
            return View(model);
        }

        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        if (product == null) return NotFound();

        // CẬP NHẬT TEXT
        product.ProductName = model.ProductName;
        product.Price = model.Price;
        product.Stock = model.Stock;
        product.Description = model.Description;
        product.IsActive = model.IsActive;
        product.CategoryId = model.CategoryId;

        // ===== XỬ LÝ ẢNH =====
        if (imageFile != null && imageFile.Length > 0)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // XÓA ẢNH CŨ
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldImage = Path.Combine(uploadPath, product.ImageUrl);
                if (System.IO.File.Exists(oldImage))
                    System.IO.File.Delete(oldImage);
            }

            // TÊN FILE MỚI
            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            product.ImageUrl = fileName;
        }

        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }


    [HttpGet("toggle/{id}")]
    public IActionResult Toggle(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();

        product.IsActive = !product.IsActive;
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("delete/{id}")]
    public IActionResult Delete(int id)
    {
        var product = _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.ProductId == id);

        if (product == null) return NotFound();

        return View(product);
    }
    // POST: admin/product/delete/5
    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();

        // ❗ XOÁ ẢNH
        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/upload",
                product.ImageUrl
            );

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        _context.Products.Remove(product);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }


    private void LoadCategories(int? selectedId = null)
    {
        var categories = _context.Categories
            .Where(c => c.IsActive)
            .ToList();

        ViewBag.CategoryId = new SelectList(
            categories,
            "CategoryId",
            "CategoryName",
            selectedId
        );
    }

}
