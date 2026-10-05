using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ServiceFlow.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ServiceFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }
    
    // POST: api/auth/login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // 1. Check username and password against the demo user in config
        if (request.Username != _config["Auth:DemoUsername"] ||
            request.Password != _config["Auth:DemoPassword"])
        {
            return Unauthorized("Wrong username or password.");
        }
        
        //2. Create the signing key from the secret in config
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        // 3. Claims = facts about the user that go inside the token
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Username)
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