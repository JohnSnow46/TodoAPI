using Microsoft.EntityFrameworkCore;
using TodoAPI.Core.Entities;
using TodoAPI.Core.Interfaces;
using TodoAPI.Infrastructure.Data;

namespace TodoAPI.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TodoDbContext _context;

        public CategoryRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return await _context.Categories
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            category.Id = Guid.NewGuid();
            category.CreatedAt = DateTime.UtcNow;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.Id);
            if (existingCategory == null)
                throw new InvalidOperationException($"Category with ID {category.Id} not found");

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            // Check if category has tasks
            var hasAssociatedTasks = await _context.Tasks.AnyAsync(t => t.CategoryId == id);
            if (hasAssociatedTasks)
            {
                // Set CategoryId to null for all tasks in this category
                var tasks = await _context.Tasks.Where(t => t.CategoryId == id).ToListAsync();
                foreach (var task in tasks)
                {
                    task.CategoryId = null;
                }
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CategoryExistsAsync(Guid id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<int> GetTaskCountByCategoryAsync(Guid categoryId)
        {
            return await _context.Tasks.CountAsync(t => t.CategoryId == categoryId);
        }
    }
}