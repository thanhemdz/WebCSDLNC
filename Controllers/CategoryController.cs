using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuiTanThanh_2280602928_W3.Data;
using BuiTanThanh_2280602928_W3.Models;


namespace BuiTanThanh_2280602928_W3.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductCategories.ToListAsync());
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategory category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCategory category)
        {
            if (id != category.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            // Kiểm tra nếu category có sản phẩm thì không cho xóa
            bool hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                TempData["ErrorMessage"] = "Không thể xóa loại danh mục này vì đang có sản phẩm thuộc loại này.";
                return RedirectToAction(nameof(Index));
            }
            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}