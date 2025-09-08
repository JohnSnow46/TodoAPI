using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Core.Entities
{
    public class TaskItem
    {
        [Required]
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Todo;
        public Enums.TaskPriority Priority { get; set; } = Enums.TaskPriority.Medium;
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public required User User { get; set; }
        public Category? Category { get; set; }
    }
}
