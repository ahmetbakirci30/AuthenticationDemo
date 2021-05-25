using AuthenticationDemo.API.Models;
using AuthenticationDemo.API.ViewModels;
using System.Threading.Tasks;

namespace AuthenticationDemo.API.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> Register(RegisterViewModel model);
        Task<UserManagerResponse> Login(LoginViewModel model);

        Task<(bool succeeded, string message)> RegisterAsync(RegisterViewModel model);
        Task<(bool succeeded, string message)> LoginAsync(LoginViewModel model);
    }
}