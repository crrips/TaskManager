using System.Security.Cryptography;
using System.Text;

namespace TaskManager.Services;

public class HashService(IConfiguration config) : IHashService
{
    public string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}