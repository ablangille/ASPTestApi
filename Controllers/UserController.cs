using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestApi.Models;
using TestApi.Data;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly TestApiDbContext dbContext;

        public UserController(TestApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await dbContext.Users!.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetOne([FromRoute] Guid id)
        {
            var user = await dbContext.Users!.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(AddUserRequest request)
        {
            var user = new User()
            {
                id = Guid.NewGuid(),
                name = request.name,
                email = request.email,
                dni = request.dni
            };

            try
            {
                await dbContext.Users!.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (
                    e is DbUpdateException
                    || e is DbUpdateConcurrencyException
                    || e is OperationCanceledException
                )
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
                }
                else
                {
                    throw;
                }
            }

            return Ok(user);
        }

        [HttpPut]
        [Route("update/{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateUserRequest request)
        {
            var user = await dbContext.Users!.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.email = request.email != null ? request.email : user.email;
            user.name = request.name != null ? request.name : user.name;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (
                    e is DbUpdateException
                    || e is DbUpdateConcurrencyException
                    || e is OperationCanceledException
                )
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
                }
                else
                {
                    throw;
                }
            }

            return Ok(user);
        }

        [HttpDelete]
        [Route("delete/{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var user = await dbContext.Users!.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            try
            {
                dbContext.Remove(user);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (
                    e is DbUpdateException
                    || e is DbUpdateConcurrencyException
                    || e is OperationCanceledException
                )
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
                }
                else
                {
                    throw;
                }
            }

            return Ok(user);
        }
    }
}
