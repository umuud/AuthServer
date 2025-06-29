using AuthServer.Core.Entities;

namespace AuthServer.Core.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Create a JWT token string for the specified user.
        /// </summary>
        string CreateToken(User user);

        /// <summary>
        /// Create Refresh Token
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
