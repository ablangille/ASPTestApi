using TestApi.Models;
using TestApi.Services;
using TestApi.Data;
using Microsoft.EntityFrameworkCore;

namespace TestApi.Repository
{
    public class UserRepository : IUserService
    {
        private readonly TestApiDbContext _dbContext;

        public UserRepository(TestApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<User> GetUsers()
        {
            try
            {
                return _dbContext.Users!.ToList();
            }
            catch
            {
                throw;
            }
        }

        public User? GetUser(Guid id)
        {
            try
            {
                return _dbContext.Users!.Find(id);
            }
            catch
            {
                throw;
            }
        }

        public User? GetUser(int dni)
        {
            try
            {
                return _dbContext.Users!.SingleOrDefault(user => user.Dni == dni);
            }
            catch
            {
                throw;
            }
        }

        public void AddUser(User user)
        {
            try
            {
                _dbContext.Users!.Add(user);
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                _dbContext.Entry(user).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public User? DeleteUser(Guid id)
        {
            try
            {
                User? user = _dbContext.Users!.Find(id);

                if (user != null)
                {
                    _dbContext.Users.Remove(user);
                    _dbContext.SaveChanges();
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
