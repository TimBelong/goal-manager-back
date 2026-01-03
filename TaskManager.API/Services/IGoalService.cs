using TaskManager.API.Models.DTOs;

namespace TaskManager.API.Services;

public interface IGoalService
{
    Task<List<GoalDto>> GetGoalsByUserAsync(Guid userId);
    Task<GoalDto?> GetGoalByIdAsync(Guid goalId, Guid userId);
    Task<GoalDto> CreateGoalAsync(Guid userId, CreateGoalRequest request);
    Task<bool> DeleteGoalAsync(Guid goalId, Guid userId);
    
    // Months
    Task<MonthDto?> AddMonthAsync(Guid goalId, Guid userId, CreateMonthRequest request);
    Task<bool> DeleteMonthAsync(Guid goalId, Guid monthId, Guid userId);
    
    // Tasks
    Task<TaskDto?> AddTaskAsync(Guid goalId, Guid monthId, Guid userId, CreateTaskRequest request);
    Task<TaskDto?> ToggleTaskAsync(Guid goalId, Guid taskId, Guid userId);
    Task<bool> DeleteTaskAsync(Guid goalId, Guid monthId, Guid taskId, Guid userId);
    
    // SubGoals
    Task<SubGoalDto?> AddSubGoalAsync(Guid goalId, Guid userId, CreateSubGoalRequest request);
    Task<SubGoalDto?> ToggleSubGoalAsync(Guid goalId, Guid subGoalId, Guid userId);
    Task<bool> DeleteSubGoalAsync(Guid goalId, Guid subGoalId, Guid userId);
    
    // Analytics
    Task<AnalyticsDto> GetAnalyticsAsync(Guid userId);
}

