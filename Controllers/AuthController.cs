using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;
using TestApi.Models;
using TestApi.Interface;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IUsers _IUsers;

        public AuthController(IUsers IUsers)
        {
            _IUsers = IUsers;
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

            return Ok("Auth successful");
        }
    }
}
