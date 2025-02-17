using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/tokens")]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public TokenController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("generate")]
    public IActionResult GenerateToken()
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var x = jwtSettings["Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }
}