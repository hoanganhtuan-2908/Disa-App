using IAM.Application.DTOs.Requests;
using IAM.Infrastructure.Data.Entities;
using IAM.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IAMService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAMDbContext _context;
        private readonly JwtTokenGenerator _jwt;
        private readonly BCryptPassworkHasher _hasher;

        public AuthController(IAMDbContext context, JwtTokenGenerator jwt, BCryptPassworkHasher hasher)
        {
            _context = context;
            _hasher = hasher;
            _jwt = jwt;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var existed = await _context.Users
                .AnyAsync(x => x.Email == request.Email);

            if (existed)
            {
                return BadRequest("Email already exists");
            }

            var customerRole = await _context.Roles
                .FirstOrDefaultAsync(x => x.Name == "Customer");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _hasher.Hash(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            if (customerRole != null)
            {
                user.Roles.Add(customerRole);
            }

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok("Register success");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
            {
                return Unauthorized();
            }

            if (!_hasher.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized();
            }

            var token = _jwt.Generate(user);

            return Ok(new
            {
                AccessToken = token
            });
        }
    }
}
