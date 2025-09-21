using ApiMyStore.Data;
using ApiMyStore.DTO.Auth;
using ApiMyStore.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMyStore.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IJwtService _jwt;

        public AuthController(ApplicationDbContext db, IJwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _db.Usuarios.AnyAsync(u => u.Username == dto.Username))
                return BadRequest(new { message = "Username ya en uso." });

            var user = new ApiMyStore.Models.Usuario
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };

            _db.Usuarios.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction(null, new { id = user.Id }, new { user.Id, user.Username, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Usuarios.SingleOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) return Unauthorized(new { message = "Credenciales inválidas." });

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Credenciales inválidas." });

            var token = _jwt.GenerateToken(user);
            return Ok(new { token, expiresIn = 3600 });
        }
    }
}