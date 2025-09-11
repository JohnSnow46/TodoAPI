using TodoAPI.Core.Enums;

namespace TodoAPI.Application.DTOs.Task
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Core.Enums.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
