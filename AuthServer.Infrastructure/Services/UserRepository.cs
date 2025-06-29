using AuthServer.Core.Entities;
using AuthServer.Core.Interfaces;
using AuthServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace AuthServer.Infrastructure.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            _logger.LogInformation("Starting GetAllAsync");
            try
            {
                var users = await _context.Users.AsNoTracking().ToListAsync();
                _logger.LogInformation("Retrieved {Count} users", users.Count());
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync");
                return Enumerable.Empty<User>();
            }
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Starting GetByIdAsync for Id: {UserId}", id);
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                    _logger.LogWarning("No user found with Id in GetByIdAsync: {UserId}", id);
                else
                    _logger.LogInformation("User found with Id: {UserId}", id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByIdAsync for Id: {UserId}", id);
                return null;
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            _logger.LogInformation("Starting GetByUsernameAsync for Username: {Username}", username);
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null)
                    _logger.LogWarning("No user found with Username in GetByUsernameAsync: {Username}", username);
                else
                    _logger.LogInformation("User found with Username: {Username}", username);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByUsernameAsync for Username: {Username}", username);
                return null;
            }
        }

        public async Task AddAsync(User user)
        {
            _logger.LogInformation("Starting AddAsync for Username: {Username}", user.UserName);
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User added successfully with Id: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddAsync for Username: {Username}", user.UserName);
                return;
            }
        }

        public async Task UpdateAsync(User user)
        {
            _logger.LogInformation("Starting UpdateAsync for UserId: {UserId}", user.Id);
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User updated successfully for Id: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for UserId: {UserId}", user.Id);
                return;
            }
        }

        public async Task DeleteAsync(User user)
        {
            _logger.LogInformation("Starting DeleteAsync for UserId: {UserId}", user.Id);
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User deleted successfully with Id: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAsync for UserId: {UserId}", user.Id);
                return;
            }
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
        }

    }
}