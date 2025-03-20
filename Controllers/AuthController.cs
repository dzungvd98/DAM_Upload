using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAM_Upload.Models;
using DAM_Upload.Dtos;
using DAM_Upload.Services;
using System.Security.Cryptography;
using System.Text;
using DAM_Upload.DTO;

namespace DAM_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DamUploadDbContext _context;
        private readonly IEmailService _emailService;

        public AuthController(DamUploadDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Username == model.Username);
            if (existingUser) return BadRequest("Username already exists.");

            var existingEmail = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (existingEmail) return BadRequest("Email already exists.");

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registration successful!");
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(new { Message = "Login successful!", User = user });
        }

      
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return BadRequest("Email does not exist.");

            
            var token = GenerateResetToken1();

           
            user.ResetToken = token;
            await _context.SaveChangesAsync();

            
            await _emailService.SendEmailAsync(request.Email, "Reset password",
                $"Your password reset token is: <b>{token}</b>");

            return Ok("Password reset token has been sent to your email.");
        }

        
        private string GenerateResetToken1()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }


        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return BadRequest("Email does not exist.");

            
            if (user.ResetToken != request.Token)
                return BadRequest("Invalid or expired token.");

            
            user.PasswordHash = HashPassword(request.NewPassword);
            user.ResetToken = null; 
            await _context.SaveChangesAsync();

            return Ok("Password was reset successfully.");
        }


       
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        
        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
        private string GenerateResetToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
