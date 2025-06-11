using AuthServer.Core.Entities;

namespace AuthServer.Core.Interfaces
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hash a plain text password for a given user.
        /// </summary>
        string HashPassword(User user, string password);

        /// <summary>
        /// Verify a provided password against the stored hash.
        /// </summary>
        bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
    }
}
