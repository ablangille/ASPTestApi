using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestApi.Models;
using TestApi.Services;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthRequest request)
        {
            try
            {
                var user = await Task.FromResult(_userService.GetUser(request.Dni));

                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (!BCryptNet.Verify(request.Password, user.Password))
                {
                    return BadRequest("Incorrect password");
                }

                //create claims details based on the user information
                var claims = new[]
                {
                    new Claim(
                        JwtRegisteredClaimNames.Sub,
                        Environment.GetEnvironmentVariable("TestApi_JWT_SUBJECT")!
                    ),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Email", user.Email!),
                    new Claim("Dni", user.Dni.ToString())
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("TestApi_JWT_KEY")!)
                );
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    Environment.GetEnvironmentVariable("TestApi_JWT_ISSUER")!,
                    Environment.GetEnvironmentVariable("TestApi_JWT_AUDIENCE")!,
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
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch(ArgumentException ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
