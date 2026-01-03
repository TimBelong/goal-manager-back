namespace TaskManager.API.Models.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Foreign key
    public Guid MonthId { get; set; }
    public Month Month { get; set; } = null!;
}

