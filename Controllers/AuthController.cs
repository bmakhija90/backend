using EcommerceAPI.Models;
using EcommerceAPI.Models.Dto;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MongoDbService _db;
        private readonly JwtService _jwt;

        public AuthController(MongoDbService db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
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
            return Ok(new AuthResponseDto { Token = token, Email = user.Email, Role = user.Role });
        }
    }

}
