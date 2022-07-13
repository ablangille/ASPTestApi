using TestApi.Models;
using Microsoft.EntityFrameworkCore;

/*
DB Context represents a session with the database, and
its used to query & save instances of modelled entities.
*/
namespace TestApi.Data
{
    public class TestApiDbContext : DbContext
    {
        public TestApiDbContext(DbContextOptions options) : base(options) { }

        // properties to interact with entities
        // dbSet is sort of like a table
        public DbSet<User>? Users { get; set; }
    }
}
