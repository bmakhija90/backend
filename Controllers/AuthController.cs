using EcommerceAPI.Models;
using EcommerceAPI.Models.Dto;
using EcommerceAPI.Repositories;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MongoDbService _db;
        private readonly JwtService _jwt;
        private readonly IProfileService _profileService;

        public AuthController(MongoDbService db, JwtService jwt,IProfileService profileService)
        {
            _db = db;
            _jwt = jwt;
            _profileService = profileService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _db.Users.Find(u => u.Email == dto.Email).AnyAsync())
                return BadRequest("Email already in use.");

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key
            };

            await _db.Users.InsertOneAsync(user);
            var newuser = await _db.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
            await _profileService.CreateProfileAsync(new CreateProfileRequest
            {
                UserId = newuser.Id,
                Email = dto.Email,
                FirstName = "",
                LastName =""
            });
            var token = _jwt.GenerateToken(user.Id, user.Email, user.Role);
            return Ok(new AuthResponseDto { Token = token, Email = user.Email, Role = user.Role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
            if (user == null) return Unauthorized();

            using var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dto.Password));
            if (!computedHash.SequenceEqual(user.PasswordHash))
                return Unauthorized();

            var token = _jwt.GenerateToken(user.Id, user.Email, user.Role);
            return Ok(new AuthResponseDto { Token = token, Email = user.Email, Role = user.Role, userId = user.Id });
        }

        [HttpGet("me")]
        public async Task<IActionResult> VerifyToken()
        {
            try
            {
                // Get token from Authorization header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("No token provided");
                }

                // Validate token using JwtService
                var principal = _jwt.ValidateToken(token);
                if (principal == null)
                {
                    return Unauthorized("Invalid token");
                }

                // Get user ID from claims
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Invalid token claims");
                }

                // Find user in database
                var user = await _db.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Return basic user info
                var userInfo = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role
                };

                return Ok(new { isValid = true, user = userInfo });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in VerifyToken: {ex.Message}");
                return Unauthorized("Token verification failed");
            }
        }

    }

}
