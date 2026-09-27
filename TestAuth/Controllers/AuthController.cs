using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Claims;
using System.Text;
using System.Text;
using TestAuth.Data;
using TestAuth.DTO_s;
using TestAuth.Models;

namespace TestAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(AuthDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public IActionResult Register(User user)
        {
         _context.Add(user);
            _context.SaveChanges();
           // return Ok(user);
                return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto login)
        {
            var user = _context.Users
                .FirstOrDefault(x =>
                    x.UserName == login.Username &&
                    x.Password == login.Password);

            if (user == null)
            {
                return Forbid("Invalid username or password");
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Ok(new
            {
                accessToken = jwt
            });
        }
    }
}
