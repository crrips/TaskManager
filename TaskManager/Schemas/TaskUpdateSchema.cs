namespace TaskManager.Schemas;

public class TaskUpdateSchema
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
}