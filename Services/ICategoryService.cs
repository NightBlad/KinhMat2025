using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shopping_Cart_2.Services
{
    public interface ICategoryService
    {
        // Lấy danh sách các danh mục dưới dạng SelectListItem để sử dụng trong dropdown
        IEnumerable<SelectListItem> GetSelectList();

        // Lấy tất cả danh mục từ cơ sở dữ liệu
        Task<IEnumerable<Category>> GetAllCategories();

        // Lấy thông tin một danh mục cụ thể theo ID
        Task<Category?> GetCategoryById(int id);

        // Thêm một danh mục mới vào cơ sở dữ liệu
        Task AddCategory(Category category);

        // Cập nhật thông tin của một danh mục đã có
        Task UpdateCategory(Category category);

        // Xóa một danh mục theo ID
        Task DeleteCategory(int id);
    }
}