namespace TaskManager.Schemas;

public class AddTaskToTagRequestSchema
{
    public int TagId { get; set; }
    public int TaskId { get; set; }
}