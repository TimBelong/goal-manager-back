namespace TaskManager.API.Models.Entities;

public enum GoalType
{
    Plan,
    SubGoals
}

public class Goal
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GoalType Type { get; set; }
    public int Year { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation properties
    public ICollection<Month> Months { get; set; } = new List<Month>();
    public ICollection<SubGoal> SubGoals { get; set; } = new List<SubGoal>();
}

