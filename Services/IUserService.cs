using taskmanagementAPI.Models;

namespace taskmanagementAPI.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> CreateUser(string username, string password);
    }
}
