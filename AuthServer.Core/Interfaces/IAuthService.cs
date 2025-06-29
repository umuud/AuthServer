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
        Task<LoginResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task RevokeTokenAsync(RevokeTokenRequest request);

    }
}
