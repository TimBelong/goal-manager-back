namespace TaskManager.API.Models.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public ICollection<DailyActivity> DailyActivities { get; set; } = new List<DailyActivity>();
}

