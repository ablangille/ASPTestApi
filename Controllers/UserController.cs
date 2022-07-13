using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Get()
        {
            return Ok(dbContext.Users!.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserRequest request)
        {
            try
            {
                var user = new User()
                {
                    id = Guid.NewGuid(),
                    name = request.name,
                    email = request.email,
                    dni = request.dni
                };

                await dbContext.Users!.AddAsync(user);
                await dbContext.SaveChangesAsync();

                return Ok(user);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
