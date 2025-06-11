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
            try
            {
                var hash = _inner.HashPassword(user, password);
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
            try
            {
                var result = _inner.VerifyHashedPassword(user, hashedPassword, providedPassword);
                var success = result == PasswordVerificationResult.Success;
                if (!success)
                    _logger.LogWarning("Password verification failed for user {Username}", user.UserName);
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