using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Schemas;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/Auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterSchema schema)
    {
        if (await _db.Users.AnyAsync(u => u.Username == schema.Username))
            return BadRequest("Username already exists");

        var user = new User
        {
            Username = schema.Username,
            PasswordHash = Hash(schema.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { user.Id, user.Username });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginSchema schema)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == schema.Username);
        if (user == null || user.PasswordHash != Hash(schema.Password))
            return Unauthorized("Invalid credentials");

        var token = _tokenService.GenerateToken(user);

        return Ok(new AuthResponseSchema { Token = token });
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}