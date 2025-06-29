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

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
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
            var accessToken = _tokenService.CreateToken(user);

            _logger.LogInformation("Generating refresh token for user {Username}", request.Username);
            var refreshToken = _tokenService.GenerateRefreshToken(request.IpAddress);

            user.RefreshTokens.Add(refreshToken);
            await _userRepo.UpdateAsync(user); // RefreshToken'ı kaydediyoruz

            _logger.LogInformation("Login successful for {Username}, tokens generated", request.Username);
            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            _logger.LogInformation("Starting refresh token process");

            var user = await _userRepo.GetUserByRefreshTokenAsync(request.RefreshToken);
            if (user == null)
            {
                _logger.LogWarning("Refresh token not found or user missing");
                throw new UnauthorizedAccessException("Geçersiz refresh token.");
            }

            var existingToken = user.RefreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);
            if (existingToken == null || !existingToken.IsActive)
            {
                _logger.LogWarning("Refresh token is inactive or revoked");
                throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş refresh token.");
            }

            // Eski token'ı revoke et
            existingToken.Revoked = DateTime.UtcNow;
            existingToken.RevokedByIp = request.IpAddress;
            var newRefreshToken = _tokenService.GenerateRefreshToken(request.IpAddress ?? "unknown");
            existingToken.ReplacedByToken = newRefreshToken.Token;

            user.RefreshTokens.Add(newRefreshToken);
            await _userRepo.UpdateAsync(user);

            var newAccessToken = _tokenService.CreateToken(user);

            return new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task RevokeTokenAsync(RevokeTokenRequest request)
        {
            _logger.LogInformation("Revoking refresh token");

            var user = await _userRepo.GetUserByRefreshTokenAsync(request.RefreshToken);
            if (user == null)
            {
                _logger.LogWarning("Token revoke failed: user not found");
                throw new UnauthorizedAccessException("Geçersiz refresh token.");
            }

            var token = user.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken);
            if (token == null || !token.IsActive)
            {
                _logger.LogWarning("Token revoke failed: token not active or already revoked");
                throw new UnauthorizedAccessException("Geçersiz veya zaten iptal edilmiş refresh token.");
            }

            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = request.IpAddress ?? "unknown";

            await _userRepo.UpdateAsync(user);

            _logger.LogInformation("Refresh token revoked successfully");
        }


    }
}