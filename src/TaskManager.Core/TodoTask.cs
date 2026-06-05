namespace TaskManager.Core;

public class TodoTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }

    public bool IsOverdue =>
        !IsCompleted && DueDate.HasValue && DueDate.Value < DateTime.UtcNow;
}
