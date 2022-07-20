using Microsoft.EntityFrameworkCore;

namespace TestApi.Models
{
    [Index(nameof(Dni), IsUnique = true)]
    public class User
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int Dni { get; set; }
    }
}
