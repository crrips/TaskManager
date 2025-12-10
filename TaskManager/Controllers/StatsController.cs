using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManager.Data;
using TaskManager.Schemas;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StatsController(AppDbContext db)
    {
        _db = db;
    }

    private int UserId => int.Parse(
        User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
        User.FindFirstValue(ClaimTypes.NameIdentifier)!
    );

    [HttpPost("GetUserStats")]
    public async Task<ActionResult<UserStatsSchema>> GetUserStats()
    {
        var tasks = await _db.Tasks
            .Where(t => t.UserId == UserId)
            .ToListAsync();

        if (tasks.Count == 0)
        {
            return new UserStatsSchema
            {
                TotalTasks = 0,
                CompletedTasks = 0,
                CompletionRate = 0,
            };
        }

        var total = tasks.Count;
        var completed = tasks.Count(t => t.IsCompleted);

        var stats = new UserStatsSchema
        {
            TotalTasks = total,
            CompletedTasks = completed,
            CompletionRate = Math.Round((double)completed / total * 100, 2),
        };

        return Ok(stats);
    }
}