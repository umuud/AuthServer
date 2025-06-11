// AuthServer.Core/Interfaces/IAuthService.cs
using System;
using System.Threading.Tasks;
using AuthServer.Application.DTOs;

namespace AuthServer.Core.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Yeni kullanıcı kaydı yapar ve oluşturulan kullanıcının Id’sini döner.
        /// </summary>
        Task<Guid> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Giriş bilgileri doğruysa JWT token’ı döner.
        /// </summary>
        Task<string> LoginAsync(LoginRequest request);
    }
}
