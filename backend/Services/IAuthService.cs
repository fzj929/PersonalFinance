using PersonalFinance.API.Models;

namespace PersonalFinance.API.Services
{
    public interface IAuthService
    {
        Task<User> Register(string username, string email, string password);
        Task<string> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}