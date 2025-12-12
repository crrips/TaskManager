using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Schemas;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, ITokenService tokenService, IHashService hashService) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequestSchema schema)
    {
        if (await db.Users.AnyAsync(u => EF.Functions.Like(u.Username, schema.Username)))
            return BadRequest("Username already exists");

        var user = new User
        {
            Username = schema.Username,
            PasswordHash = hashService.Hash(schema.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok(new { user.Id, user.Username });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestSchema schema)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == schema.Username);
        if (user == null || user.PasswordHash != hashService.Hash(schema.Password))
            return Unauthorized("Invalid credentials");

        var token = tokenService.GenerateToken(user);

        return Ok(new { Token = token });
    }
}