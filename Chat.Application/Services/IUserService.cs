using System.Threading.Tasks;

namespace Chat.Application.Services
{
    public interface IUserService
    {
        bool CreateUser(string userName, string password);
        Task<bool> CreateUserAsync(string userName, string password);
        Task<bool> EditUserAsync(string userName, string password);
    }
}
