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
public class TagsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TagsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("CreateTag")]
    public async Task<ActionResult<TagSchema>> Create(CreateTagSchema schema)
    {
        var tag = new Tag { Name = schema.Name };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        return Ok(new TagSchema { Id = tag.Id, Name = tag.Name });
    }
    
    [HttpPost("DeleteTag/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id);
        if (tag == null) return NotFound();
        
        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("GetAllTags")]
    public async Task<ActionResult<List<TagSchema>>> GetAll()
    {
        var tags = await _db.Tags
            .Select(t => new TagSchema
            {
                Id = t.Id,
                Name = t.Name
            })
            .ToListAsync();

        return Ok(tags);
    }
    
    [HttpPost("{tagId}/AddTaskToTag/{taskId}")]
    public async Task<IActionResult> AddTaskToTag(int tagId, int taskId)
    {
        var tag = await _db.Tags
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(t => t.Id == tagId);

        if (tag == null)
            return NotFound("Tag not found");

        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null)
            return NotFound("Task not found");

        if (!tag.Tasks.Contains(task))
            tag.Tasks.Add(task);

        await _db.SaveChangesAsync();
        return Ok();
    }


    [HttpPost("{tagId}/RemoveTaskFromTag/{taskId}")]
    public async Task<IActionResult> RemoveTaskFromTag(int tagId, int taskId)
    {
        var tag = await _db.Tags
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(t => t.Id == tagId);

        if (tag == null)
            return NotFound("Tag not found");

        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null)
            return NotFound("Task not found");

        if (tag.Tasks.Contains(task))
            tag.Tasks.Remove(task);

        await _db.SaveChangesAsync();
        return Ok();
    }
}