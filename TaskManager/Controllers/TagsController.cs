using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Schemas;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagsController(AppDbContext db) : AuthorizedController
{
    [HttpPost("CreateTag")]
    public async Task<ActionResult> Create([FromBody] CreateTagRequestSchema schema)
    {
        var tag = new Tag { Name = schema.Name };
        db.Tags.Add(tag);
        await db.SaveChangesAsync();

        return Ok(new { Id = tag.Id, Name = tag.Name });
    }
    
    [HttpPost("DeleteTag")]
    public async Task<IActionResult> Delete([FromBody] int id)
    {
        var tag = await db.Tags.FirstOrDefaultAsync(t => t.Id == id);
        if (tag == null) return NotFound();
        
        db.Tags.Remove(tag);
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("GetAllTags")]
    public async Task<ActionResult> GetAll()
    {
        var tags = await db.Tags
            .Select(t => new
            {
                t.Id,
                t.Name
            })
            .ToListAsync();

        return Ok(tags);
    }
    
    [HttpPost("AddTaskToTag")]
    public async Task<IActionResult> AddTaskToTag([FromBody] AddTaskToTagRequestSchema schema)
    {
        var tag = await db.Tags
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(t => t.Id == schema.TagId);

        if (tag == null)
            return NotFound("Tag not found");

        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == schema.TaskId);
        if (task == null)
            return NotFound("Task not found");

        if (!tag.Tasks.Contains(task))
            tag.Tasks.Add(task);

        await db.SaveChangesAsync();
        return Ok();
    }


    [HttpPost("RemoveTaskFromTag")]
    public async Task<IActionResult> RemoveTaskFromTag([FromBody] RemoveTaskFromTagRequestSchema schema)
    {
        var tag = await db.Tags
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(t => t.Id == schema.TagId);

        if (tag == null)
            return NotFound("Tag not found");

        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == schema.TaskId);
        if (task == null)
            return NotFound("Task not found");

        if (tag.Tasks.Contains(task))
            tag.Tasks.Remove(task);

        await db.SaveChangesAsync();
        return Ok();
    }
}