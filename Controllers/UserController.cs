using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;
using TestApi.Models;
using TestApi.Interface;
using TestApi.Handlers;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IUsers _IUsers;

        public UserController(IUsers IUsers)
        {
            _IUsers = IUsers;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return Ok(await Task.FromResult(_IUsers.GetUsers()));
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<User>> GetOne([FromRoute] Guid id)
        {
            var user = await Task.FromResult(_IUsers.GetUser(id));

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
            var user = await Task.FromResult(_IUsers.GetUser(dni));

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
                id = Guid.NewGuid(),
                name = request.name,
                email = request.email,
                password = BCryptNet.HashPassword(request.password),
                dni = request.dni
            };

            try
            {
                _IUsers.AddUser(user);
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
            var user = await Task.FromResult(_IUsers.GetUser(id));

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.email = request.email != null ? request.email : user.email;
            user.name = request.name != null ? request.name : user.name;

            // hash and save on new password
            if (request.password != null && !BCryptNet.Verify(request.password, user.password))
            {
                user.password = BCryptNet.HashPassword(request.password);
            }

            try
            {
                _IUsers.UpdateUser(user);
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
                var user = await Task.FromResult(_IUsers.DeleteUser(id));

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
