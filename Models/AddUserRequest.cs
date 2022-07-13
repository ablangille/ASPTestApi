namespace TestApi.Models
{
    public class AddUserRequest
    {
        public string? name { get; set; }
        public string? email { get; set; }
        public int dni { get; set; }
    }
}
