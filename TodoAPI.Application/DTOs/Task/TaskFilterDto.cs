using TodoAPI.Core.Enums;

namespace TodoAPI.Application.DTOs.Task
{
    public class TaskFilterDto
    {
        public Core.Enums.TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public Guid? CategoryId { get; set; }
        public string? Search { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
