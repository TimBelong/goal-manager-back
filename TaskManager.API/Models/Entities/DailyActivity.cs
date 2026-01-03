namespace TaskManager.API.Models.Entities;

public class DailyActivity
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public int TasksCompleted { get; set; }

    // Foreign key
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

