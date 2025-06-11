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
            try
            {
                var userId = await _authService.RegisterAsync(req);
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
            try
            {
                var token = await _authService.LoginAsync(req);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Login failed for {Username}", req.Username);
                return Unauthorized(ex.Message);
            }
        }
    }
}