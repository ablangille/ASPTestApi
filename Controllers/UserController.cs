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
    }
}
