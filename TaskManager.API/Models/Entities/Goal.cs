namespace TaskManager.API.Models.Entities;

public enum GoalType
{
    Plan,
    SubGoals,
    Savings
}


public enum GoalCategory
{
    PersonalDevelopment,
    Career,
    Finance,
    Health,
    Sport,
    Nutrition,
    Relationships,
    Habits,
    Travel,
    Other
}

public class Goal
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GoalType Type { get; set; }
    public GoalCategory Category { get; set; }
    public int Year { get; set; }
    public decimal? TargetAmount { get; set; }
    public decimal? CurrentAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation properties
    public ICollection<Month> Months { get; set; } = new List<Month>();
    public ICollection<SubGoal> SubGoals { get; set; } = new List<SubGoal>();
}

