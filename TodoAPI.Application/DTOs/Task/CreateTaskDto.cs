using System.ComponentModel.DataAnnotations;
using TodoAPI.Core.Enums;

namespace TodoAPI.Application.DTOs.Task
{
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required")]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public Guid? CategoryId { get; set; }
    }
}
