using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shopping_Cart_2.Services
{
    public interface ICategoryService
    {
        IEnumerable<SelectListItem> GetSelectList();
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category?> GetCategoryById(int id);
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int id);
    }
}
