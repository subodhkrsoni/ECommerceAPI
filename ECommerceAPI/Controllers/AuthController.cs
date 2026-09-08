using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        // =========================
        // REGISTER
        // =========================
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDto dto)
        {
            // Check duplicate username or email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == dto.Username ||
                    u.Email == dto.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    message = "Username or Email already exists"
                });
            }


            // =========================
            // Create User
            // =========================
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };


            // =========================
            // Secure Password Hashing
            // =========================
            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                dto.Password);


            // =========================
            // Save User
            // =========================
            _context.Users.Add(user);

            await _context.SaveChangesAsync();


            // =========================
            // Response
            // =========================
            return CreatedAtAction(
                nameof(Register),
                new { id = user.Id },
                new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    createdAt = user.CreatedAt
                });
        }


        // =========================
        // LOGIN
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            // =========================
            // Find User
            // =========================
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == dto.Username);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }


            // =========================
            // Verify Password
            // =========================
            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.Password);


            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }


            // =========================
            // JWT Claims
            // =========================
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };


            // =========================
            // JWT Security Key
            // =========================
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));


            // =========================
            // Signing Credentials
            // =========================
            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);


            // =========================
            // Token Expiration
            // =========================
            var expireMinutes =
                int.TryParse(
                    _configuration["Jwt:ExpireMinutes"],
                    out var minutes)
                ? minutes
                : 60;


            // =========================
            // Create JWT Token
            // =========================
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    expireMinutes),
                signingCredentials: credentials
            );


            // =========================
            // Convert Token to String
            // =========================
            var jwtToken =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);


            // =========================
            // Response
            // =========================
            return Ok(new
            {
                message = "Login successful",

                token = jwtToken,

                expiresInMinutes = expireMinutes,

                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role
                }
            });
        }
    }
}