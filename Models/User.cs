using Microsoft.EntityFrameworkCore;

namespace TestApi.Models
{
    [Index(nameof(dni), IsUnique = true)]
    public class User
    {
        public Guid id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public int dni { get; set; }
    }
}
