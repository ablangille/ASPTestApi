using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;
using TestApi.Models;
using TestApi.Services;
using TestApi.Handlers;

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
            return Ok(await Task.FromResult(_userService.GetUsers()));
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<User>> GetOne([FromRoute] Guid id)
        {
            var user = await Task.FromResult(_userService.GetUser(id));

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }

        [HttpGet]
        [Route("{dni:int}")]
        public async Task<ActionResult<User>> GetOne([FromRoute] int dni)
        {
            var user = await Task.FromResult(_userService.GetUser(dni));

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
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
            catch (Exception e)
            {
                if (e is DbUpdateException dbUpdateEx)
                {
                    var result = DbUpdateExceptionHandler.HandleDbUpdateException(dbUpdateEx);
                    return StatusCode(result.statusCode, result.message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
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
            catch (Exception e)
            {
                if (e is DbUpdateException dbUpdateEx)
                {
                    var result = DbUpdateExceptionHandler.HandleDbUpdateException(dbUpdateEx);
                    return StatusCode(result.statusCode, result.message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
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
            catch (Exception e)
            {
                if (e is DbUpdateException dbUpdateEx)
                {
                    var result = DbUpdateExceptionHandler.HandleDbUpdateException(dbUpdateEx);
                    return StatusCode(result.statusCode, result.message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
