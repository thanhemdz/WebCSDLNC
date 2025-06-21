using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuiTanThanh_2280602928_W3.Data;
using BuiTanThanh_2280602928_W3.Models;
using BuiTanThanh_2280602928_W3.ViewModels;

namespace BuiTanThanh_2280602928_W3.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var categories = await _context.ProductCategories.ToListAsync();
            var products = await _context.Products.Include(p => p.Category).Where(p => p.Status == "active").ToListAsync();
            var viewModel = new ProductIndexViewModel();
            if (categoryId.HasValue)
            {
                var cat = categories.FirstOrDefault(c => c.Id == categoryId.Value);
                if (cat != null)
                {
                    viewModel.Groups.Add(new ProductCategoryGroupViewModel
                    {
                        CategoryId = cat.Id,
                        CategoryName = cat.Name,
                        Products = products.Where(p => p.CategoryId == cat.Id).ToList()
                    });
                    viewModel.SelectedCategoryId = cat.Id;
                }
            }
            else
            {
                foreach (var cat in categories)
                {
                    viewModel.Groups.Add(new ProductCategoryGroupViewModel
                    {
                        CategoryId = cat.Id,
                        CategoryName = cat.Name,
                        Products = products.Where(p => p.CategoryId == cat.Id).Take(4).ToList()
                    });
                }
            }
            return View(viewModel);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.ProductCategories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? ImageFile)
        {
            // Kiểm tra trùng tên trong cùng danh mục
            bool isDuplicate = _context.Products.Any(p => p.Name == product.Name && p.CategoryId == product.CategoryId);
            if (isDuplicate)
            {
                ModelState.AddModelError("Name", "Tên sản phẩm đã tồn tại trong danh mục này.");
            }
            // Kiểm tra giá > 0
            if (product.Price <= 0)
            {
                ModelState.AddModelError("Price", "Giá sản phẩm phải lớn hơn 0.");
            }
            if (ModelState.IsValid)
            {
                // Xử lý upload file ảnh nếu có
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    product.ImageUrl = "/images/" + fileName;
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.ProductCategories.ToList();
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = _context.ProductCategories.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImageFile)
        {
            if (id != product.Id) return NotFound();
            // Kiểm tra trùng tên trong cùng danh mục (trừ chính nó)
            bool isDuplicate = _context.Products.Any(p => p.Id != id && p.Name == product.Name && p.CategoryId == product.CategoryId);
            if (isDuplicate)
            {
                ModelState.AddModelError("Name", "Tên sản phẩm đã tồn tại trong danh mục này.");
            }
            // Kiểm tra giá > 0
            if (product.Price <= 0)
            {
                ModelState.AddModelError("Price", "Giá sản phẩm phải lớn hơn 0.");
            }
            if (ModelState.IsValid)
            {
                var oldProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (oldProduct == null) return NotFound();
                // Xử lý upload file ảnh nếu có
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    product.ImageUrl = "/images/" + fileName;
                }
                else
                {
                    // Nếu không upload ảnh mới, giữ nguyên ảnh cũ
                    product.ImageUrl = oldProduct.ImageUrl;
                }
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.ProductCategories.ToList();
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                bool hasOrder = _context.OrderItems.Any(oi => oi.ProductId == id);
                if (hasOrder)
                {
                    TempData["DeleteMessage"] = "Sản phẩm đã từng bán, bạn không thể xóa. Nếu muốn ngừng kinh doanh, hãy chỉnh trạng thái ở phần Sửa.";
                    return RedirectToAction(nameof(Delete), new { id });
                }
                else
                {
                    _context.Products.Remove(product);
                    TempData["DeleteMessage"] = "Xóa sản phẩm thành công.";
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Display(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
        public async Task<IActionResult> AdminList()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }
    }
}
