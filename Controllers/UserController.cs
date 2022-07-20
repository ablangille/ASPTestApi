using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;
using TestApi.Models;
using TestApi.Services;

namespace TestApi.Controllers
{
    [Authorize] // this routes need an access token
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            try
            {
                var users = await Task.FromResult(_userService.GetUsers());

                return Ok(users);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<User>> GetOne([FromRoute] Guid id)
        {
            try
            {
                var user = await Task.FromResult(_userService.GetUser(id));

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
        }

        [HttpGet]
        [Route("{dni:int}")]
        public async Task<ActionResult<User>> GetOne([FromRoute] int dni)
        {
            try
            {
                var user = await Task.FromResult(_userService.GetUser(dni));

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<User>> Add(AddUserRequest request)
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Password = BCryptNet.HashPassword(request.Password),
                Dni = request.Dni
            };

            try
            {
                _userService.AddUser(user);
                await Task.FromResult(user);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.InnerException?.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }

            return Ok(user);
        }

        [HttpPut]
        [Route("update/{id:guid}")]
        public async Task<ActionResult<User>> Update([FromRoute] Guid id, UpdateUserRequest request)
        {
            var user = await Task.FromResult(_userService.GetUser(id));

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Email = request.Email != null ? request.Email : user.Email;
            user.Name = request.Name != null ? request.Name : user.Name;

            // hash and save on new password
            if (request.Password != null && !BCryptNet.Verify(request.Password, user.Password))
            {
                user.Password = BCryptNet.HashPassword(request.Password);
            }

            try
            {
                _userService.UpdateUser(user);
                await Task.FromResult(user);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }

            return Ok(user);
        }

        [HttpDelete]
        [Route("delete/{id:guid}")]
        public async Task<ActionResult<User>> Delete([FromRoute] Guid id)
        {
            try
            {
                var user = await Task.FromResult(_userService.DeleteUser(id));

                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound("User not found");
                }
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
        }
    }
}
