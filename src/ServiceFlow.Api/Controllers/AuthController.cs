using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ServiceFlow.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServiceFlow.Core.Interfaces;

namespace ServiceFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUserService _userService;

    public AuthController(IConfiguration config, IUserService userService)
    {
        _config = config;
        _userService = userService;
    }
    
    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. Check username and password against the users table
        var user = await _userService.ValidateCredentialsAsync(request.Username, request.Password);
        if (user is null)
        {
            return Unauthorized("Wrong username or password.");
        }
        
        //2. Create the signing key from the secret in config
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        // 3. Claims = facts about the user that go inside the token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };
        
        // 4. Build the token
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);
        
        // 5. Return the token as a string 
        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

}