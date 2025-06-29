using AuthServer.Core.Entities;
using AuthServer.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthServer.Infrastructure.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<JwtTokenService> _logger;

        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public JwtTokenService(IConfiguration config, ILogger<JwtTokenService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string CreateToken(User user)
        {
            _logger.LogInformation("Starting token creation for user {UserId}", user.Id);
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.UtcNow.AddHours(1);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation("Token created successfully for user {UserId}", user.Id);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating token for user {UserId}", user.Id);
                return string.Empty;
            }
        }

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var randomBytes = new byte[64];
            _rng.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7), // 7 gün geçerli
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}