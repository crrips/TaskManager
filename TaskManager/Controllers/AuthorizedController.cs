using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Controllers;

public abstract class AuthorizedController: ControllerBase
{
    protected int UserId => int.Parse(
        User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
        User.FindFirstValue(ClaimTypes.NameIdentifier)!
    );
}