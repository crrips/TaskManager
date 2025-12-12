using TaskManager.Models;

namespace TaskManager.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}