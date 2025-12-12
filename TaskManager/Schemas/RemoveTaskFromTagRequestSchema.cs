namespace TaskManager.Schemas;

public class RemoveTaskFromTagRequestSchema
{
    public int TagId { get; set; }
    public int TaskId { get; set; }
}