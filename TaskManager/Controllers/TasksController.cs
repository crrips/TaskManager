using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Schemas;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/Tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;
    public TasksController(AppDbContext db) => _db = db;

    private int UserId => int.Parse(
        User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
        User.FindFirstValue(ClaimTypes.NameIdentifier)!
    );

    [HttpPost("GetMyTasks")]
    public async Task<IActionResult> GetMyTasks()
    {
        var tasks = await _db.Tasks
            .Where(t => t.UserId == UserId)
            .Select(t => new TaskResponseSchema
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted
            })
            .ToListAsync();

        return Ok(tasks);
    }

    [HttpPost("CreateTask")]
    public async Task<IActionResult> Create(TaskCreateSchema schema)
    {
        var task = new TaskItem
        {
            Title = schema.Title,
            Description = schema.Description,
            UserId = UserId
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return Ok(new TaskResponseSchema
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted
        });
    }

    [HttpPost("UpdateTask/{id}")]
    public async Task<IActionResult> Update(int id, TaskUpdateSchema schema)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (task == null) return NotFound();

        task.Title = schema.Title;
        task.Description = schema.Description;
        task.IsCompleted = schema.IsCompleted;

        await _db.SaveChangesAsync();

        return Ok(new TaskResponseSchema
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted
        });
    }

    [HttpPost("DeleteTask/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (task == null) return NotFound();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return Ok();
    }
}
