using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Schemas;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatsController(AppDbContext db) : AuthorizedController
{
    [HttpPost("GetUserStats")]
    public async Task<ActionResult> GetUserStats([FromBody] UserStatsRequestSchema schema)
    {
        var tasks = await db.Tasks
            .Where(t => t.UserId == UserId)
            .ToListAsync();

        if (tasks.Count == 0)
        {
            var emptyTasks = new
            {
                TotalTasks = 0,
                CompletedTasks = 0,
                CompletionRate = 0,
            };
            return Ok(emptyTasks);
        }

        var total = tasks.Count;
        var completed = tasks.Count(t => t.IsCompleted);

        var stats = new
        {
            TotalTasks = total,
            CompletedTasks = completed,
            CompletionRate = Math.Round((double)completed / total * 100, 2),
        };

        return Ok(stats);
    }
}