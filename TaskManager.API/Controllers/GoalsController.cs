using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Models.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GoalsController : ControllerBase
{
    private readonly IGoalService _goalService;

    public GoalsController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet]
    public async Task<ActionResult<List<GoalDto>>> GetGoals()
    {
        var userId = GetUserId();
        var goals = await _goalService.GetGoalsByUserAsync(userId);
        return Ok(goals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GoalDto>> GetGoal(Guid id)
    {
        var userId = GetUserId();
        var goal = await _goalService.GetGoalByIdAsync(id, userId);
        
        if (goal == null)
            return NotFound();
        
        return Ok(goal);
    }

    [HttpPost]
    public async Task<ActionResult<GoalDto>> CreateGoal([FromBody] CreateGoalRequest request)
    {
        var userId = GetUserId();
        var goal = await _goalService.CreateGoalAsync(userId, request);
        return CreatedAtAction(nameof(GetGoal), new { id = goal.Id }, goal);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGoal(Guid id)
    {
        var userId = GetUserId();
        var result = await _goalService.DeleteGoalAsync(id, userId);
        
        if (!result)
            return NotFound();
        
        return NoContent();
    }

    // Months
    [HttpPost("{goalId}/months")]
    public async Task<ActionResult<MonthDto>> AddMonth(Guid goalId, [FromBody] CreateMonthRequest request)
    {
        var userId = GetUserId();
        var month = await _goalService.AddMonthAsync(goalId, userId, request);
        
        if (month == null)
            return BadRequest(new { message = "Could not add month" });
        
        return Ok(month);
    }

    [HttpDelete("{goalId}/months/{monthId}")]
    public async Task<IActionResult> DeleteMonth(Guid goalId, Guid monthId)
    {
        var userId = GetUserId();
        var result = await _goalService.DeleteMonthAsync(goalId, monthId, userId);
        
        if (!result)
            return NotFound();
        
        return NoContent();
    }

    // Tasks
    [HttpPost("{goalId}/months/{monthId}/tasks")]
    public async Task<ActionResult<TaskDto>> AddTask(Guid goalId, Guid monthId, [FromBody] CreateTaskRequest request)
    {
        var userId = GetUserId();
        var task = await _goalService.AddTaskAsync(goalId, monthId, userId, request);
        
        if (task == null)
            return BadRequest(new { message = "Could not add task" });
        
        return Ok(task);
    }

    [HttpPatch("{goalId}/tasks/{taskId}/toggle")]
    public async Task<ActionResult<TaskDto>> ToggleTask(Guid goalId, Guid taskId)
    {
        var userId = GetUserId();
        var task = await _goalService.ToggleTaskAsync(goalId, taskId, userId);
        
        if (task == null)
            return NotFound();
        
        return Ok(task);
    }

    [HttpDelete("{goalId}/months/{monthId}/tasks/{taskId}")]
    public async Task<IActionResult> DeleteTask(Guid goalId, Guid monthId, Guid taskId)
    {
        var userId = GetUserId();
        var result = await _goalService.DeleteTaskAsync(goalId, monthId, taskId, userId);
        
        if (!result)
            return NotFound();
        
        return NoContent();
    }

    // SubGoals
    [HttpPost("{goalId}/subgoals")]
    public async Task<ActionResult<SubGoalDto>> AddSubGoal(Guid goalId, [FromBody] CreateSubGoalRequest request)
    {
        var userId = GetUserId();
        var subGoal = await _goalService.AddSubGoalAsync(goalId, userId, request);
        
        if (subGoal == null)
            return BadRequest(new { message = "Could not add subgoal" });
        
        return Ok(subGoal);
    }

    [HttpPatch("{goalId}/subgoals/{subGoalId}/toggle")]
    public async Task<ActionResult<SubGoalDto>> ToggleSubGoal(Guid goalId, Guid subGoalId)
    {
        var userId = GetUserId();
        var subGoal = await _goalService.ToggleSubGoalAsync(goalId, subGoalId, userId);
        
        if (subGoal == null)
            return NotFound();
        
        return Ok(subGoal);
    }

    [HttpDelete("{goalId}/subgoals/{subGoalId}")]
    public async Task<IActionResult> DeleteSubGoal(Guid goalId, Guid subGoalId)
    {
        var userId = GetUserId();
        var result = await _goalService.DeleteSubGoalAsync(goalId, subGoalId, userId);
        
        if (!result)
            return NotFound();
        
        return NoContent();
    }
}

