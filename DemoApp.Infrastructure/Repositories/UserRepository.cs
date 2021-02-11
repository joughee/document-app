using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DemoApp.Core.Entities;
using DemoApp.Infrastructure.DataAccess;
using DemoApp.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using DemoApp.Core.Interfaces;

namespace DemoApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        //private Configuration.DataAccess _connStrings;
        private IDataAccessManager _dataAccess;

        public UserRepository(IDataAccessManager dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task<User> GetUserById(int id)
        {
            return await _dataAccess.GetUserById(id);
        }
        public async Task<User> GetUserByUserName(string username)
        {
            var user = await _dataAccess.GetUserByUserName(username);
            user.Roles = await _dataAccess.GetUserRoles(user.Id);

            return user;
        }
        public async Task<int> CreateUser(User user)
        {
            return await _dataAccess.CreateUser(user);
        }

        public async Task<bool> DeleteUser(int userId)
        {
            return await _dataAccess.DeleteUser(userId);
        }

        public async Task<bool> UpdateUser(User user)
        {
            return await _dataAccess.UpdateUser(user);
        }
        public async Task<bool> AddUserToRole(int userId, string role)
        {
            return await _dataAccess.InsertUserRole(userId, role);
        }
        public async Task<bool> RemoveUserFromRole(int userId, string role)
        {
            return await _dataAccess.DeleteUserRole(userId, role);
        }
    }
}
