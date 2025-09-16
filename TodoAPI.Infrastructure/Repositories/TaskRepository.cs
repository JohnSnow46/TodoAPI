using Microsoft.EntityFrameworkCore;
using TodoAPI.Core.Entities;
using TodoAPI.Core.Enums;
using TodoAPI.Core.Interfaces;
using TodoAPI.Infrastructure.Data;

namespace TodoAPI.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TodoDbContext _context;

        public TaskRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Include(t => t.Category)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem> AddTaskAsync(TaskItem task)
        {
            task.Id = Guid.NewGuid();
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(task.Id) ?? task;
        }

        public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {
            var existingTask = await _context.Tasks.FindAsync(task.Id);
            if (existingTask == null)
                throw new InvalidOperationException($"Task with ID {task.Id} not found");

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.Priority = task.Priority;
            existingTask.CategoryId = task.CategoryId;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetTaskByIdAsync(task.Id) ?? existingTask;
        }

        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(Guid userId)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserAndStatusAsync(Guid userId, Core.Enums.TaskStatus status)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserAndPriorityAsync(Guid userId, TaskPriority priority)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Priority == priority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserAndCategoryAsync(Guid userId, Guid? categoryId)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.CategoryId == categoryId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Where(t => t.CategoryId == categoryId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(TaskPriority priority)
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Include(t => t.Category)
                .Where(t => t.Priority == priority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Core.Enums.TaskStatus status)
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Include(t => t.Category)
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(DateTime currentDate)
        {
            var overdueDate = currentDate.AddDays(-7);
            return await _context.Tasks
                .Include(t => t.User)
                .Include(t => t.Category)
                .Where(t => t.Status != Core.Enums.TaskStatus.Done && t.CreatedAt < overdueDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetTaskCountByUserAsync(Guid userId)
        {
            return await _context.Tasks
                .CountAsync(t => t.UserId == userId);
        }

        public async Task<int> GetCompletedTaskCountByUserAsync(Guid userId)
        {
            return await _context.Tasks
                .CountAsync(t => t.UserId == userId && t.Status == Core.Enums.TaskStatus.Done);
        }
    }
}
