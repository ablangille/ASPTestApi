using TestApi.Models;

namespace TestApi.Interface
{
    public interface IUsers
    {
        public List<User> GetUsers();
        public User? GetUser(Guid id);
        public User? GetUser(int dni);
        public void AddUser(User user);
        public void UpdateUser(User user);
        public User? DeleteUser(Guid id);
    }
}
