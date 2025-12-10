namespace TaskManager.Models;

public class Tag
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public string Name { get; set; } = null!;
    
    public List<TaskItem> Tasks { get; set; } = new();
}
