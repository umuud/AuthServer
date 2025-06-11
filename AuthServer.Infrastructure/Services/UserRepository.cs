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
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                    _logger.LogWarning("No user found with Id in GetByIdAsync: {UserId}", id);
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
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null)
                    _logger.LogWarning("No user found with Username in GetByUsernameAsync: {Username}", username);
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
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddAsync for Username: {Username}", user.UserName);
                return;
            }
        }

        public async Task UpdateAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for UserId: {UserId}", user.Id);
                return;
            }
        }

        public async Task DeleteAsync(User user)
        {
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAsync for UserId: {UserId}", user.Id);
                return;
            }
        }
    }
}