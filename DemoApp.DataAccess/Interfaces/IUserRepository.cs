using DemoApp.Core.Entities;
using System;
using System.Threading.Tasks;

namespace DemoApp.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int id);
        Task<User> GetUserByUserName(string userName);
        Task<int> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
        Task<bool> AddUserToRole(int userId, string role);
        Task<bool> RemoveUserFromRole(int userId, string role);
    }
}