using TaskStatus = TodoAPI.Core.Enums.TaskStatus;

namespace TodoAPI.Application.DTOs.Task
{
    public class UpdateTaskStatusDto
    {
        public TaskStatus Status { get; set; }
    }
}
