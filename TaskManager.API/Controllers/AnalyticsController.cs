using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Models.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IGoalService _goalService;

    public AnalyticsController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet("activity")]
    public async Task<ActionResult<AnalyticsDto>> GetAnalytics()
    {
        var userId = GetUserId();
        var analytics = await _goalService.GetAnalyticsAsync(userId);
        return Ok(analytics);
    }
}

