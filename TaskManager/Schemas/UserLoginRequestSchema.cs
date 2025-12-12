namespace TaskManager.Schemas;

public class UserLoginRequestSchema
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}