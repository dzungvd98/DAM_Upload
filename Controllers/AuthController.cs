using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAM_Upload.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DAM_Upload.DTO;
using DAM_Upload.Service;
using Microsoft.AspNetCore.Identity.Data;

namespace DAM_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DamUploadDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(DamUploadDbContext context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
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

            var token = GenerateJwtToken(user);
            return Ok(new { Message = "Login successful!", Token = token, User = user });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Lấy token từ header Authorization
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("No token provided.");
            }

            // Kiểm tra token có hợp lệ không
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token))
            {
                return BadRequest("Invalid token.");
            }

            var jwtToken = tokenHandler.ReadJwtToken(token);
            var expiryDate = jwtToken.ValidTo;

            // Thêm token vào danh sách blacklist
            var blacklistedToken = new BlacklistedToken
            {
                Token = token,
                BlacklistedAt = DateTime.UtcNow,
                ExpiryDate = expiryDate
            };

            _context.BlacklistedTokens.Add(blacklistedToken);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Logout successful" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO request)
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
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
        }



        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        //    if (user == null) return BadRequest("Email does not exist.");


        //    if (user.ResetToken != request.Token)
        //        return BadRequest("Invalid or expired token.");


        //    user.PasswordHash = HashPassword(request.NewPassword);
        //    user.ResetToken = null;
        //    await _context.SaveChangesAsync();

        //    return Ok("Password was reset successfully.");
        //}



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
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
