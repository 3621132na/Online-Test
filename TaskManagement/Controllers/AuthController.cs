using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Data;
using TaskManagement.DTOs;
using TaskManagement.Models;

namespace TaskManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TaskManagementDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(TaskManagementDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        if (await _context.users.AnyAsync(u => u.username == dto.Username || u.email == dto.Email))
            return BadRequest("Username or email already exists.");
        var user = new user
        {
            username = dto.Username,
            passwordhash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            email = dto.Email,
            createdat = DateTime.Now,
        };
        _context.users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await _context.users.SingleOrDefaultAsync(u => u.username == dto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.passwordhash))
            return Unauthorized("Invalid credentials.");
        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
    private string GenerateJwtToken(user user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        Console.WriteLine($"Generating token with Jwt Key: {jwtKey}");
        Console.WriteLine($"Jwt Issuer: {jwtIssuer}");
        Console.WriteLine($"Jwt Audience: {jwtAudience}");
        if (string.IsNullOrEmpty(jwtKey))
            throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json");
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            new Claim(ClaimTypes.Name, user.username),
            new Claim(ClaimTypes.Role, user.role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine($"Generated token: {tokenString}");
        return tokenString;
    }
}