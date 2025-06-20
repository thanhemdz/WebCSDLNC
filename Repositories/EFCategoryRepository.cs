using BuiTanThanh_2280602928_W3.Data;
using BuiTanThanh_2280602928_W3.Models;
using Microsoft.EntityFrameworkCore;

namespace BuiTanThanh_2280602928_W3.Repositories
{
    public class EFCategoryRepository 
    {
        private readonly ApplicationDbContext _context;

        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thêm một category mới vào cơ sở dữ liệu
        public async Task AddAsync(ProductCategory category)
        {
            await _context.ProductCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        // Xóa một category theo ID
        public async Task DeleteAsync(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category != null)
            {
                _context.ProductCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        // Lấy tất cả các categories
        public async Task<IEnumerable<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        // Lấy một category theo ID
        public async Task<ProductCategory> GetByIdAsync(int id)
        {
            return await _context.ProductCategories
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Cập nhật một category trong cơ sở dữ liệu
        public async Task UpdateAsync(ProductCategory category)
        {
            var existingCategory = await _context.ProductCategories.FindAsync(category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                // Cập nhật các thuộc tính khác nếu cần
                await _context.SaveChangesAsync();
            }
        }
    }
}
