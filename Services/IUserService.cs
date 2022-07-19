using TestApi.Models;

namespace TestApi.Services
{
    public interface IUserService
    {
        public List<User> GetUsers();
        public User? GetUser(Guid id);
        public User? GetUser(int dni);
        public void AddUser(User user);
        public void UpdateUser(User user);
        public User? DeleteUser(Guid id);
    }
}
