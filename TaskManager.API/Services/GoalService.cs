using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Models.DTOs;
using TaskManager.API.Models.Entities;

namespace TaskManager.API.Services;

public class GoalService : IGoalService
{
    private readonly AppDbContext _context;

    public GoalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GoalDto>> GetGoalsByUserAsync(Guid userId)
    {
        var goals = await _context.Goals
            .Where(g => g.UserId == userId)
            .Include(g => g.Months)
                .ThenInclude(m => m.Tasks)
            .Include(g => g.SubGoals)
            .OrderByDescending(g => g.Year)
            .ThenByDescending(g => g.CreatedAt)
            .ToListAsync();

        return goals.Select(MapToDto).ToList();
    }

    public async Task<GoalDto?> GetGoalByIdAsync(Guid goalId, Guid userId)
    {
        var goal = await _context.Goals
            .Where(g => g.Id == goalId && g.UserId == userId)
            .Include(g => g.Months)
                .ThenInclude(m => m.Tasks)
            .Include(g => g.SubGoals)
            .FirstOrDefaultAsync();

        return goal == null ? null : MapToDto(goal);
    }

    public async Task<GoalDto> CreateGoalAsync(Guid userId, CreateGoalRequest request)
    {
        var goal = new Goal
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Type = request.Type.ToLower() == "plan" ? GoalType.Plan : GoalType.SubGoals,
            Year = request.Year ?? DateTime.UtcNow.Year,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Goals.Add(goal);
        await _context.SaveChangesAsync();

        return MapToDto(goal);
    }

    public async Task<GoalDto?> UpdateGoalAsync(Guid goalId, Guid userId, UpdateGoalRequest request)
    {
        var goal = await _context.Goals
            .Where(g => g.Id == goalId && g.UserId == userId)
            .Include(g => g.Months)
                .ThenInclude(m => m.Tasks)
            .Include(g => g.SubGoals)
            .FirstOrDefaultAsync();

        if (goal == null) return null;

        goal.Title = request.Title;
        goal.Description = request.Description;

        await _context.SaveChangesAsync();

        return MapToDto(goal);
    }

    public async Task<bool> DeleteGoalAsync(Guid goalId, Guid userId)
    {
        var goal = await _context.Goals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

        if (goal == null) return false;

        _context.Goals.Remove(goal);
        await _context.SaveChangesAsync();
        return true;
    }

    // Months
    public async Task<MonthDto?> AddMonthAsync(Guid goalId, Guid userId, CreateMonthRequest request)
    {
        var goal = await _context.Goals
            .Include(g => g.Months)
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

        if (goal == null || goal.Type != GoalType.Plan) return null;

        // Check if month already exists
        if (goal.Months.Any(m => m.Name == request.Name)) return null;

        var month = new Month
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Order = request.Order,
            GoalId = goalId
        };

        _context.Months.Add(month);
        await _context.SaveChangesAsync();

        return new MonthDto(month.Id, month.Name, month.Order, new List<TaskDto>());
    }

    public async Task<bool> DeleteMonthAsync(Guid goalId, Guid monthId, Guid userId)
    {
        var month = await _context.Months
            .Include(m => m.Goal)
            .FirstOrDefaultAsync(m => m.Id == monthId && m.GoalId == goalId && m.Goal.UserId == userId);

        if (month == null) return false;

        _context.Months.Remove(month);
        await _context.SaveChangesAsync();
        return true;
    }

    // Tasks
    public async Task<TaskDto?> AddTaskAsync(Guid goalId, Guid monthId, Guid userId, CreateTaskRequest request)
    {
        var month = await _context.Months
            .Include(m => m.Goal)
            .FirstOrDefaultAsync(m => m.Id == monthId && m.GoalId == goalId && m.Goal.UserId == userId);

        if (month == null) return null;

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            Completed = false,
            MonthId = monthId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return new TaskDto(task.Id, task.Text, task.Completed, task.CompletedAt);
    }

    public async Task<TaskDto?> ToggleTaskAsync(Guid goalId, Guid taskId, Guid userId)
    {
        var task = await _context.Tasks
            .Include(t => t.Month)
                .ThenInclude(m => m.Goal)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Month.GoalId == goalId && t.Month.Goal.UserId == userId);

        if (task == null) return null;

        task.Completed = !task.Completed;
        task.CompletedAt = task.Completed ? DateTime.UtcNow : null;

        // Record daily activity
        if (task.Completed)
        {
            await RecordActivityAsync(userId);
        }

        await _context.SaveChangesAsync();

        return new TaskDto(task.Id, task.Text, task.Completed, task.CompletedAt);
    }

    public async Task<bool> DeleteTaskAsync(Guid goalId, Guid monthId, Guid taskId, Guid userId)
    {
        var task = await _context.Tasks
            .Include(t => t.Month)
                .ThenInclude(m => m.Goal)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.MonthId == monthId && 
                                      t.Month.GoalId == goalId && t.Month.Goal.UserId == userId);

        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    // SubGoals
    public async Task<SubGoalDto?> AddSubGoalAsync(Guid goalId, Guid userId, CreateSubGoalRequest request)
    {
        var goal = await _context.Goals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

        if (goal == null || goal.Type != GoalType.SubGoals) return null;

        var subGoal = new SubGoal
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            Completed = false,
            GoalId = goalId
        };

        _context.SubGoals.Add(subGoal);
        await _context.SaveChangesAsync();

        return new SubGoalDto(subGoal.Id, subGoal.Text, subGoal.Completed, subGoal.CompletedAt);
    }

    public async Task<SubGoalDto?> ToggleSubGoalAsync(Guid goalId, Guid subGoalId, Guid userId)
    {
        var subGoal = await _context.SubGoals
            .Include(sg => sg.Goal)
            .FirstOrDefaultAsync(sg => sg.Id == subGoalId && sg.GoalId == goalId && sg.Goal.UserId == userId);

        if (subGoal == null) return null;

        subGoal.Completed = !subGoal.Completed;
        subGoal.CompletedAt = subGoal.Completed ? DateTime.UtcNow : null;

        // Record daily activity
        if (subGoal.Completed)
        {
            await RecordActivityAsync(userId);
        }

        await _context.SaveChangesAsync();

        return new SubGoalDto(subGoal.Id, subGoal.Text, subGoal.Completed, subGoal.CompletedAt);
    }

    public async Task<bool> DeleteSubGoalAsync(Guid goalId, Guid subGoalId, Guid userId)
    {
        var subGoal = await _context.SubGoals
            .Include(sg => sg.Goal)
            .FirstOrDefaultAsync(sg => sg.Id == subGoalId && sg.GoalId == goalId && sg.Goal.UserId == userId);

        if (subGoal == null) return false;

        _context.SubGoals.Remove(subGoal);
        await _context.SaveChangesAsync();
        return true;
    }

    // Analytics
    public async Task<AnalyticsDto> GetAnalyticsAsync(Guid userId)
    {
        var goals = await _context.Goals
            .Where(g => g.UserId == userId)
            .Include(g => g.Months)
                .ThenInclude(m => m.Tasks)
            .Include(g => g.SubGoals)
            .ToListAsync();

        var activity = await _context.DailyActivities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Date)
            .Take(365)
            .ToListAsync();

        var totalGoals = goals.Count;
        
        var allTasks = goals
            .Where(g => g.Type == GoalType.Plan)
            .SelectMany(g => g.Months)
            .SelectMany(m => m.Tasks)
            .ToList();
        
        var allSubGoals = goals
            .Where(g => g.Type == GoalType.SubGoals)
            .SelectMany(g => g.SubGoals)
            .ToList();

        var completedTasks = allTasks.Count(t => t.Completed) + allSubGoals.Count(sg => sg.Completed);
        var totalTasks = allTasks.Count + allSubGoals.Count;

        // Calculate streak
        var currentStreak = CalculateStreak(activity);

        var activityDtos = activity
            .Select(a => new DailyActivityDto(a.Date.ToString("yyyy-MM-dd"), a.TasksCompleted))
            .ToList();

        return new AnalyticsDto(activityDtos, totalGoals, completedTasks, totalTasks, currentStreak);
    }

    private async Task RecordActivityAsync(Guid userId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existing = await _context.DailyActivities
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == today);

        if (existing != null)
        {
            existing.TasksCompleted++;
        }
        else
        {
            _context.DailyActivities.Add(new DailyActivity
            {
                Id = Guid.NewGuid(),
                Date = today,
                UserId = userId,
                TasksCompleted = 1
            });
        }
    }

    private static int CalculateStreak(List<DailyActivity> activity)
    {
        if (activity.Count == 0) return 0;

        var streak = 0;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var currentDate = today;

        foreach (var day in activity.OrderByDescending(a => a.Date))
        {
            if (day.Date == currentDate || day.Date == currentDate.AddDays(-1))
            {
                streak++;
                currentDate = day.Date;
            }
            else if (day.Date < currentDate.AddDays(-1))
            {
                break;
            }
        }

        return streak;
    }

    private static GoalDto MapToDto(Goal goal)
    {
        var months = goal.Type == GoalType.Plan
            ? goal.Months.OrderBy(m => m.Order).Select(m => new MonthDto(
                m.Id,
                m.Name,
                m.Order,
                m.Tasks.Select(t => new TaskDto(t.Id, t.Text, t.Completed, t.CompletedAt)).ToList()
            )).ToList()
            : null;

        var subGoals = goal.Type == GoalType.SubGoals
            ? goal.SubGoals.Select(sg => new SubGoalDto(sg.Id, sg.Text, sg.Completed, sg.CompletedAt)).ToList()
            : null;

        var progress = CalculateProgress(goal);

        return new GoalDto(
            goal.Id,
            goal.Title,
            goal.Description,
            goal.Type == GoalType.Plan ? "plan" : "subgoals",
            goal.Year,
            goal.CreatedAt,
            months,
            subGoals,
            progress
        );
    }

    private static int CalculateProgress(Goal goal)
    {
        if (goal.Type == GoalType.Plan)
        {
            var tasks = goal.Months.SelectMany(m => m.Tasks).ToList();
            if (tasks.Count == 0) return 0;
            return (int)Math.Round(100.0 * tasks.Count(t => t.Completed) / tasks.Count);
        }
        else
        {
            if (goal.SubGoals.Count == 0) return 0;
            return (int)Math.Round(100.0 * goal.SubGoals.Count(sg => sg.Completed) / goal.SubGoals.Count);
        }
    }
}

