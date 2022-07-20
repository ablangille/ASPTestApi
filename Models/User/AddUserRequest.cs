namespace TestApi.Models
{
    public class AddUserRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int Dni { get; set; }
    }
}
