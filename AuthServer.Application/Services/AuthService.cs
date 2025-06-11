// ---------------------------
using AuthServer.Application.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthServer.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepo,
            IPasswordHasher hasher,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _userRepo = userRepo;
            _hasher = hasher;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Guid> RegisterAsync(RegisterRequest request)
        {
            _logger.LogInformation("Starting registration for {Username}", request.Username);
            if (await _userRepo.GetByUsernameAsync(request.Username) != null)
            {
                _logger.LogWarning("Registration failed: username {Username} already exists", request.Username);
                throw new InvalidOperationException("Kullanıcı adı zaten kayıtlı.");
            }

            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Hashing password for user {Username}", request.Username);
            user.PasswordHash = _hasher.HashPassword(user, request.Password);
            _logger.LogInformation("Password hashed successfully for {Username}", request.Username);

            await _userRepo.AddAsync(user);
            _logger.LogInformation("User registered successfully with Id {UserId}", user.Id);

            return user.Id;
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("Starting login for {Username}", request.Username);
            var user = await _userRepo.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed: username {Username} not found", request.Username);
                throw new UnauthorizedAccessException("Geçersiz kullanıcı adı veya şifre.");
            }

            _logger.LogInformation("Verifying password for user {Username}", request.Username);
            if (!_hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password))
            {
                _logger.LogWarning("Login failed: invalid password for {Username}", request.Username);
                throw new UnauthorizedAccessException("Geçersiz kullanıcı adı veya şifre.");
            }

            _logger.LogInformation("Creating JWT token for user {Username}", request.Username);
            var token = _tokenService.CreateToken(user);
            _logger.LogInformation("Login successful for {Username}, token generated", request.Username);
            return token;
        }
    }
}