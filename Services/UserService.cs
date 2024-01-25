using taskmanagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using taskmanagementAPI.Data;

namespace taskmanagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly TaskManagementContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(TaskManagementContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //authenticate users method
        public async Task<User> Authenticate(string username, string password)
        {
            _logger.LogInformation($"Authenticating user: {username}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username); //search for user in db
            if (user == null)
            {
                _logger.LogWarning($"User not found: {username}");
                throw new KeyNotFoundException($"User not found: {username}");
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) 
            {
                _logger.LogWarning($"Invalid password for user: {username}");
                throw new UnauthorizedAccessException("Invalid password");
            }

            _logger.LogInformation($"User authenticated: {username}"); //sucess
            return user;
        }
        //create users method
        public async Task<User> CreateUser(string username, string password)
        {
            _logger.LogInformation($"Creating user: {username}");

            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                _logger.LogWarning($"Username '{username}' already exists.");
                throw new InvalidOperationException($"Username '{username}' already exists.");
            }
            //generate salt and pass for a guiven passwd
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User '{username}' created successfully.");
            return newUser;
        }

        //create new hash for guiven pwd
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        //verify if guivenHash = storedHash
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA256(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                    {
                        _logger.LogWarning($"Password verification failed at byte {i}.");
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
