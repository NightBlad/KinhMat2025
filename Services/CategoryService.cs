using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart_2.Data;

namespace Shopping_Cart_2.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _dbContext;

        // Khởi tạo dịch vụ với ApplicationDbContext để tương tác với cơ sở dữ liệu
        public CategoryService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Lấy danh sách các danh mục dưới dạng SelectListItem để sử dụng trong dropdown
        public IEnumerable<SelectListItem> GetSelectList()
        {
            return _dbContext.Categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }) // Chuyển đổi danh mục thành SelectListItem
                .OrderBy(c => c.Text) // Sắp xếp theo tên danh mục
                .AsNoTracking() // Không theo dõi thay đổi để tối ưu hiệu suất
                .ToList(); // Trả về danh sách
        }

        // Lấy tất cả danh mục từ cơ sở dữ liệu
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _dbContext.Categories
                .AsNoTracking() // Không theo dõi thay đổi để tăng hiệu suất
                .ToListAsync(); // Trả về danh sách các danh mục dưới dạng bất đồng bộ
        }

        // Lấy thông tin danh mục theo ID
        public async Task<Category?> GetCategoryById(int id)
        {
            return await _dbContext.Categories.FindAsync(id); // Tìm danh mục theo ID và trả về (có thể null nếu không tìm thấy)
        }

        // Thêm một danh mục mới vào cơ sở dữ liệu
        public async Task AddCategory(Category category)
        {
            _dbContext.Categories.Add(category); // Thêm danh mục vào tập hợp categories
            await _dbContext.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
        }

        // Cập nhật thông tin một danh mục đã có
        public async Task UpdateCategory(Category category)
        {
            _dbContext.Categories.Update(category); // Cập nhật thông tin danh mục
            await _dbContext.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
        }

        // Xóa một danh mục theo ID
        public async Task DeleteCategory(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id); // Tìm danh mục theo ID
            if (category != null) // Kiểm tra xem danh mục có tồn tại không
            {
                _dbContext.Categories.Remove(category); // Xóa danh mục khỏi tập hợp
                await _dbContext.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
            }
        }
    }
}