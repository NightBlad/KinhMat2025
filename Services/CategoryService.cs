using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart_2.Data;

namespace Shopping_Cart_2.Services
{
    
public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<SelectListItem> GetSelectList()
        {
            return _dbContext.categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
               .OrderBy(c => c.Text)
               .AsNoTracking()
               .ToList();
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _dbContext.categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            return await _dbContext.categories.FindAsync(id);
        }

        public async Task AddCategory(Category category)
        {
            _dbContext.categories.Add(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            _dbContext.categories.Update(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCategory(int id)
        {
            var category = await _dbContext.categories.FindAsync(id);
            if (category != null)
            {
                _dbContext.categories.Remove(category);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
