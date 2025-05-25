// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using ventixe_user_service.Data;
using ventixe_user_service.Models;

namespace ventixe_user_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserDbContext _context;

        public AuthController(UserDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                // Hitta användaren
                var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

                if (user == null)
                {
                    return Unauthorized(new { message = "Användaren finns inte" });
                }

                // Kontrollera lösenord
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Fel lösenord" });
                }

                // Skapa en enkel token (för demo)
                var token = $"demo-token-{user.Id}-{DateTime.UtcNow.Ticks}";

                var response = new AuthResponse
                {
                    Token = token,
                    User = user,
                    Message = "Inloggning lyckades!"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server error: {ex.Message}" });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Kolla om email redan finns
                if (_context.Users.Any(u => u.Email == request.Email))
                {
                    return BadRequest(new { message = "E-post redan registrerad" });
                }

                // Skapa ny användare
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = "User"
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                var token = $"demo-token-{user.Id}-{DateTime.UtcNow.Ticks}";

                var response = new AuthResponse
                {
                    Token = token,
                    User = user,
                    Message = "Registrering lyckades!"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server error: {ex.Message}" });
            }
        }

        // Test endpoint
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "User Service API fungerar!", timestamp = DateTime.UtcNow });
        }
    }
}