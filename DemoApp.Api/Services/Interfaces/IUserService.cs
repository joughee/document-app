using DemoApp.Api.Models;
using DemoApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApp.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        Task<User> GetUserById(int id);
        Task<User> GetUserByUserName(string userName);
        Task<int?> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
        Task<bool> AddUserToRole(int userId, string role);
        Task<bool> RemoveUserFromRole(int userId, string role);
    }
}
