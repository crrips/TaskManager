namespace TaskManager.Schemas;

public class TaskCreateSchema
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}