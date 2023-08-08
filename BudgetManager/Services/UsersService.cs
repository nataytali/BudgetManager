using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BudgetManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BudgetManager.Controllers;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace BudgetManager.Services
{
    public interface IUserService
    {
        bool IsAnExistingUser(string userName);
        bool IsValidUserCredentials(string userName, string password);
        string GetUserRole(string userName);
    }

    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationContext _context;

        private List<UserModel> _users;
        // inject database here for user validation
        public UserService(ILogger<UserService> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
            _users = _context.Users.ToList<UserModel>();

        }
        public async Task<List<UserModel>> Get()
        {
            _users = await _context.Users.ToListAsync();
            return _users;
        }
        public bool IsValidUserCredentials(string userName, string password)
        {
            _logger.LogInformation($"Validating user [{userName}]");
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            var res = _users.Find(u => u.Email == userName) == null ? false : true; 
            if (res)
            {
                IdentityUser user = _users.Find(u => u.Email == userName);
                var hasher = new PasswordHasher<IdentityUser>();
                var hashResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if(hashResult == PasswordVerificationResult.Success)
                    return true;
            }

            return false;
        }

        public bool IsAnExistingUser(string userName)
        {
            return _users.Contains(_users.FindLast(u => u.Email == userName));
        }

        public string GetUserRole(string userName)
        {
            if (!IsAnExistingUser(userName))
            {
                return string.Empty;
            }

            if (userName == "admin")
            {
                return UserRoles.Admin;
            }

            return UserRoles.BasicUser;
        }
    }

    public static class UserRoles
    {
        public const string Admin = nameof(Admin);
        public const string BasicUser = nameof(BasicUser);
    }
}