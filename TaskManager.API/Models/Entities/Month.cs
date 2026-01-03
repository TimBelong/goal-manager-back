namespace TaskManager.API.Models.Entities;

public class Month
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }

    // Foreign key
    public Guid GoalId { get; set; }
    public Goal Goal { get; set; } = null!;

    // Navigation properties
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}

