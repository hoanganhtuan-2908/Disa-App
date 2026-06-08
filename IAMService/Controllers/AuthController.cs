using IAM.Application.DTOs.Requests;
using IAM.Infrastructure.Data.Entities;
using IAM.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IAMService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAMDbContext _context;
    private readonly JwtTokenGenerator _jwt;
    private readonly BCryptPassworkHasher _hasher;

    public AuthController(
        IAMDbContext context,
        JwtTokenGenerator jwt,
        BCryptPassworkHasher hasher)
    {
        _context = context;
        _jwt = jwt;
        _hasher = hasher;
    }

    // CUSTOMER REGISTER
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var emailExists = await _context.Users
            .AnyAsync(x => x.Email == request.Email);

        if (emailExists)
            return BadRequest("Email đã tồn tại");

        var usernameExists = await _context.Users
            .AnyAsync(x => x.Username == request.Username);

        if (usernameExists)
            return BadRequest("Tên đăng nhập đã tồn tại");

        var customerRole = await _context.Roles
            .FirstOrDefaultAsync(x => x.Name == "Customer");

        if (customerRole == null)
            return BadRequest("Không tìm thấy vai trò Customer");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _hasher.Hash(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.Roles.Add(customerRole);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return Ok("Đăng ký tài khoản thành công");
    }

    // LOGIN
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == request.Username);

        if (user == null)
            return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

        var accessToken = _jwt.Generate(user);

        var refreshToken = Guid.NewGuid().ToString();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Đăng nhập thành công",
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    // REFRESH TOKEN
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenRequest request)
    {
        var refresh = await _context.RefreshTokens
            .Include(x => x.User)
            .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

        if (refresh == null)
            return Unauthorized("Refresh Token không hợp lệ");

        if (refresh.ExpiresAt < DateTime.UtcNow)
            return Unauthorized("Refresh Token đã hết hạn");

        if (refresh.RevokedAt != null)
            return Unauthorized("Refresh Token đã bị thu hồi");

        var newAccessToken = _jwt.Generate(refresh.User);

        return Ok(new
        {
            Message = "Làm mới Access Token thành công",
            AccessToken = newAccessToken
        });
    }

    // THÔNG TIN TÀI KHOẢN
    
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            Email = User.FindFirst(ClaimTypes.Email)?.Value,

            Roles = User.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value)
        });
    }

    // ĐỔI MẬT KHẨU
    
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound("Không tìm thấy người dùng");

        if (!_hasher.Verify(
            request.OldPassword,
            user.PasswordHash))
        {
            return BadRequest("Mật khẩu cũ không chính xác");
        }

        user.PasswordHash =
            _hasher.Hash(request.NewPassword);

        await _context.SaveChangesAsync();

        return Ok("Đổi mật khẩu thành công");
    }

    
    // QUÊN MẬT KHẨU
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            return Ok(
                "Nếu email tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu sẽ được gửi");
        }

        var token = Guid.NewGuid().ToString();

        _context.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Tạo mã đặt lại mật khẩu thành công",
            ResetToken = token
        });
    }

    // ĐẶT LẠI MẬT KHẨU
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordRequest request)
    {
        var reset = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == request.Token);

        if (reset == null)
            return BadRequest("Token không hợp lệ");

        if (reset.User == null)
            return BadRequest("Không tìm thấy người dùng");

        if (reset.ExpiresAt < DateTime.UtcNow)
            return BadRequest("Token đã hết hạn");

        if (reset.RevokedAt != null)
            return BadRequest("Token đã được sử dụng");

        reset.User.PasswordHash =
            _hasher.Hash(request.NewPassword);

        reset.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok("Đặt lại mật khẩu thành công");
    }

    // ĐĂNG XUẤT
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        LogoutRequest request)
    {
        var refresh = await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.Token == request.RefreshToken);

        if (refresh == null)
            return NotFound("Không tìm thấy Refresh Token");

        refresh.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok("Đăng xuất thành công");
    }
}