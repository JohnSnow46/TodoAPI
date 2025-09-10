using TodoAPI.Core.Entities;

namespace TodoAPI.Core.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(Guid id);
        Task<Category?> GetCategoryByNameAsync(string name);
        Task<Category> AddCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<bool> CategoryExistsAsync(Guid id);
        Task<int> GetTaskCountByCategoryAsync(Guid categoryId);
    }
}
