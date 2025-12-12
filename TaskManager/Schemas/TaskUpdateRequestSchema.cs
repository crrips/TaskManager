using System.ComponentModel.DataAnnotations;

namespace TaskManager.Schemas;

public class TaskUpdateRequestSchema
{
    [Required]
    public int Id { get; set; }
    
    [MinLength(10, ErrorMessage = "title_min_len_exception")]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public bool IsCompleted { get; set; }
}