using System;

namespace DesktopTasks.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    }

    public enum TaskPriority
    {
        Low,
        Normal,
        High
    }
}
