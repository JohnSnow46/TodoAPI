using TodoAPI.Core.Enums;

namespace TodoAPI.Application.DTOs.Task
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public Guid? CategoryId { get; set; }
    }

}
