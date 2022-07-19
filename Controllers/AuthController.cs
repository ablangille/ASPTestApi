using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestApi.Models;
using TestApi.Interface;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IUsers _IUsers;
        public IConfiguration _configuration;

        public AuthController(IUsers IUsers, IConfiguration config)
        {
            _IUsers = IUsers;
            _configuration = config;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthRequest request)
        {
            var user = await Task.FromResult(_IUsers.GetUser(request.dni));

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!BCryptNet.Verify(request.password, user.password))
            {
                return BadRequest("Incorrect password");
            }

            //create claims details based on the user information
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWT:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", user.id.ToString()),
                new Claim("email", user.email!),
                new Claim("dni", user.dni.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["JWT:Issuer"],
                _configuration["JWT:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn
            );

            return Ok(
                new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                }
            );
        }
    }
}
