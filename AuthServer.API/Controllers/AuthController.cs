using AuthServer.Application.DTOs;
using AuthServer.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            _logger.LogInformation("Register attempt for {Username}", req.Username);
            try
            {
                var userId = await _authService.RegisterAsync(req);
                _logger.LogInformation("Registration successful for {Username}, UserId: {UserId}", req.Username, userId);
                return CreatedAtAction(nameof(Register), new { id = userId }, new { id = userId });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Register failed for {Username}", req.Username);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            req.IpAddress = ipAddress; // IP adresini ekledik

            _logger.LogInformation("Login attempt for {Username}", req.Username);
            try
            {
                var token = await _authService.LoginAsync(req);
                _logger.LogInformation("Login successful for {Username}", req.Username);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Login failed for {Username}", req.Username);
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest req)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            req.IpAddress = ipAddress;

            _logger.LogInformation("Refresh token attempt");

            try
            {
                var result = await _authService.RefreshTokenAsync(req);
                _logger.LogInformation("Refresh token successful");
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Refresh token failed");
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RevokeTokenRequest req)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            req.IpAddress = ipAddress;

            _logger.LogInformation("Logout attempt");

            try
            {
                await _authService.RevokeTokenAsync(req);
                _logger.LogInformation("Logout successful");
                return Ok(new { message = "Çıkış yapıldı, token iptal edildi." });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Logout failed");
                return Unauthorized(ex.Message);
            }
        }


    }
}