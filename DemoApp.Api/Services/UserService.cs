using DemoApp.Api.Models;
using DemoApp.Api.Services.Interfaces;
using DemoApp.Core.Entities;
using DemoApp.Core.Interfaces;
using DemoApp.Infrastructure.Configuration;
using DemoApp.Infrastructure.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Api.Services
{
    public class UserService : IUserService
    {

        private readonly ApiSettings _appSettings;
        private IUserRepository _userRepo;

        public UserService(IOptions<ApiSettings> appSettings, IUserRepository userRepo)
        {
            _appSettings = appSettings.Value;
            _userRepo = userRepo;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var user = await _userRepo.GetUserByUserName(model.Username);

            if (user != null && PasswordUtility.CheckPassword(user.PasswordHash, model.Password))
            {
                var token = generateJwtToken(user);
                return new AuthenticateResponse(user, token);
            }
            else
            {
                return null;
            }
        }

        public async Task<User> GetUserById(int id)
        {
            return await _userRepo.GetUserById(id);
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            return await _userRepo.GetUserByUserName(userName);
        }

        public async Task<int?> CreateUser(User user)
        {
            user.PasswordHash = PasswordUtility.EncryptPassword(user.Password, _appSettings.HashIterations);
            return await _userRepo.CreateUser(user);
        }

        public async Task<bool> UpdateUser(User user)
        {
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.PasswordHash = PasswordUtility.EncryptPassword(user.Password, _appSettings.HashIterations);
            }
            return await _userRepo.UpdateUser(user);
        }

        public async Task<bool> DeleteUser(int id)
        {
            return await _userRepo.DeleteUser(id);
        }

        public async Task<bool> AddUserToRole(int userId, string role)
        {
            return await _userRepo.AddUserToRole(userId, role);
        }

        public async Task<bool> RemoveUserFromRole(int userId, string role)
        {
            return await _userRepo.RemoveUserFromRole(userId, role);
        }

        // helpers

        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.AuthKey);
            var claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Id.ToString()));
            if (user.Roles.Any())
            {
                foreach(var role in user.Roles)
                {
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimIdentity,
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
