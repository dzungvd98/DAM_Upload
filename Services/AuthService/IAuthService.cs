using DAM_Upload.Models;

namespace DAM_Upload.Services.AuthService
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string username, string password, string email);
        Task<User> LoginAsync(string username, string password);
        Task<bool> ResetPasswordAsync(User user, string newPassword);
        Task<User> GetUserByEmailAsync(string email);
    }
}
