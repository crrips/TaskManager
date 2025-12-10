namespace TaskManager.Schemas;

public class UserStatsSchema
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public double CompletionRate { get; set; }
}