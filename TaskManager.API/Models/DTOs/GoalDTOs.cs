using System.ComponentModel.DataAnnotations;

namespace TaskManager.API.Models.DTOs;

// Request DTOs
public record CreateGoalRequest(
    [Required][MaxLength(200)] string Title,
    [MaxLength(1000)] string? Description,
    [Required] string Type, // "plan" or "subgoals"
    int? Year
);

public record CreateMonthRequest(
    [Required][MaxLength(50)] string Name,
    [Required] int Order
);

public record CreateTaskRequest(
    [Required][MaxLength(500)] string Text
);

public record CreateSubGoalRequest(
    [Required][MaxLength(500)] string Text
);

public record UpdateGoalRequest(
    [Required][MaxLength(200)] string Title,
    [MaxLength(1000)] string? Description
);

// Response DTOs
public record GoalDto(
    Guid Id,
    string Title,
    string? Description,
    string Type,
    int Year,
    DateTime CreatedAt,
    List<MonthDto>? Months,
    List<SubGoalDto>? SubGoals,
    int Progress
);

public record MonthDto(
    Guid Id,
    string Name,
    int Order,
    List<TaskDto> Tasks
);

public record TaskDto(
    Guid Id,
    string Text,
    bool Completed,
    DateTime? CompletedAt
);

public record SubGoalDto(
    Guid Id,
    string Text,
    bool Completed,
    DateTime? CompletedAt
);

public record DailyActivityDto(
    string Date,
    int TasksCompleted
);

public record AnalyticsDto(
    List<DailyActivityDto> Activity,
    int TotalGoals,
    int CompletedTasks,
    int TotalTasks,
    int CurrentStreak
);

