using TodoAPI.Core.Entities;
using TodoAPI.Core.Enums;

namespace TodoAPI.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(Guid id);
        Task<TaskItem> AddTaskAsync(TaskItem task);
        Task<TaskItem> UpdateTaskAsync(TaskItem task);
        Task<bool> DeleteTaskAsync(Guid id);

        // User-specific operations
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(Guid userId);
        Task<IEnumerable<TaskItem>> GetTasksByUserAndStatusAsync(Guid userId, Enums.TaskStatus status);
        Task<IEnumerable<TaskItem>> GetTasksByUserAndPriorityAsync(Guid userId, TaskPriority priority);
        Task<IEnumerable<TaskItem>> GetTasksByUserAndCategoryAsync(Guid userId, Guid? categoryId);

        // Additional filtering
        Task<IEnumerable<TaskItem>> GetTasksByCategoryIdAsync(Guid categoryId);
        Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(TaskPriority priority);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Enums.TaskStatus status);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(DateTime currentDate);

        // Statistics
        Task<int> GetTaskCountByUserAsync(Guid userId);
        Task<int> GetCompletedTaskCountByUserAsync(Guid userId);
    }
}

