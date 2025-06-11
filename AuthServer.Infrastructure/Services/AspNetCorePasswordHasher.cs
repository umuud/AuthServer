using AuthServer.Core.Entities;
using AuthServer.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthServer.Infrastructure.Services
{
    public class AspNetCorePasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<User> _inner = new();
        private readonly ILogger<AspNetCorePasswordHasher> _logger;
        public AspNetCorePasswordHasher(ILogger<AspNetCorePasswordHasher> logger)
        {
            _logger = logger;
        }

        public string HashPassword(User user, string password)
        {
            _logger.LogInformation("Hashing password for user {Username}", user.UserName);
            try
            {
                var hash = _inner.HashPassword(user, password);
                _logger.LogInformation("Password hashed successfully for user {Username}", user.UserName);
                return hash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hashing password for user {Username}", user.UserName);
                return string.Empty;
            }
        }

        public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            _logger.LogInformation("Verifying password for user {Username}", user.UserName);
            try
            {
                var result = _inner.VerifyHashedPassword(user, hashedPassword, providedPassword);
                var success = result == PasswordVerificationResult.Success;
                if (success)
                {
                    _logger.LogInformation("Password verification succeeded for user {Username}", user.UserName);
                }
                else
                {
                    _logger.LogWarning("Password verification failed for user {Username}", user.UserName);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password for user {Username}", user.UserName);
                return false;
            }
        }
    }
}