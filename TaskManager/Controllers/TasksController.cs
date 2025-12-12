using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Schemas;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/Tasks")]
[Authorize]
public class TasksController(AppDbContext db) : AuthorizedController
{
    [HttpPost("GetMyTasks")]
    public async Task<IActionResult> GetMyTasks(CancellationToken ct)
    {
        var tasks = await db.Tasks
            .Where(t => t.UserId == UserId)
            .AsNoTracking()
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.IsCompleted
            })
            .ToListAsync(ct);

        return Ok(tasks);
    }
    
    [HttpPost("CreateTask")]
    public async Task<IActionResult> Create(TaskCreateRequestSchema schema, CancellationToken ct)
    {
        await using var trx = await db.Database.BeginTransactionAsync(ct);

        var task = new TaskItem
        {
            Title = schema.Title,
            Description = schema.Description,
            UserId = UserId
        };

        db.Tasks.Add(task);

        await db.SaveChangesAsync(ct);

        await trx.CommitAsync(ct);

        return Ok(new
        {
            task.Id,
            task.Title,
            task.Description,
            task.IsCompleted
        });
    }
    
    [HttpPost("UpdateTask")]
    public async Task<IActionResult> Update([FromBody] TaskUpdateRequestSchema schema, CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(3000);

        var ct = cts.Token;

        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == schema.Id && t.UserId == UserId, ct);
        if (task == null) return NotFound();

        task.Title = schema.Title;
        task.Description = schema.Description;
        task.IsCompleted = schema.IsCompleted;

        await db.SaveChangesAsync(ct);

        return Ok(new
        {
            task.Id,
            task.Title,
            task.Description,
            task.IsCompleted
        });
    }
    
    [HttpPost("DeleteTask")]
    public async Task<IActionResult> Delete([FromBody] int id, CancellationToken ct)
    {
        await using var trx = await db.Database.BeginTransactionAsync(ct);

        var task = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId, ct);

        if (task == null)
            return NotFound();

        if (!task.IsCompleted)
        {
            task.IsCompleted = true;
            await db.SaveChangesAsync(ct);

            await trx.CommitAsync(ct);

            return Ok(new { status = "completed" });
        }
        else
        {
            db.Tasks.Remove(task);
            await db.SaveChangesAsync(ct);

            await trx.CommitAsync(ct);

            return Ok(new { status = "deleted" });
        }
    }
}
