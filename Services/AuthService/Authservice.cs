using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DAM_Upload.Models;
using Microsoft.EntityFrameworkCore;

namespace DAM_Upload.Services
{
    public class Authservice
    {
        private readonly DamUploadDbContext _context;

        public Authservice(DamUploadDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterAsync(string username, string password, string email)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email))
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Email = email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || user.PasswordHash != HashPassword(password))
                return null;

            return user;
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ResetPasswordAsync(User user, string newPassword)
        {
            if (user == null || string.IsNullOrWhiteSpace(newPassword))
                return false;

            user.PasswordHash = HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
