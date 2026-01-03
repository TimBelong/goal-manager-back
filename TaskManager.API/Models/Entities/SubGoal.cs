namespace TaskManager.API.Models.Entities;

public class SubGoal
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Foreign key
    public Guid GoalId { get; set; }
    public Goal Goal { get; set; } = null!;
}

