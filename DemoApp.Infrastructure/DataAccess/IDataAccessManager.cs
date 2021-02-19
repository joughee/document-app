using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Threading;
using DemoApp.Core.Entities;

namespace DemoApp.Infrastructure.DataAccess
{
    public interface IDataAccessManager
    {
        //IDbConnection _dbMainConnection;

        //IDbTransaction DbTransaction { get; set; }
        //public DataAccessManager(IDbConnection connection, IDbTransaction transaction)
        //{
        //    _dbConnection = new NpgsqlConnection(connection.ConnectionString);
        //    _dbTransaction = new Npgsql.NpgsqlTransaction();
        //}
        //public DataAccessManager(IDbConnection connection)
        //{
        //    _dbConnection = connection;
        //    //_dbTransaction = null;
        //}

        //public IEnumerable<Document> GetDocumentsByUserId(int userId)
        //{
        //    using(_dbConnection)
        //    {
        //        _dbConnection.
        //    }
        //}
        Task<int?> CreateUser(User user);
        Task<User> GetUserById(int id);
        Task<User> GetUserByUserName(string username);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
        Task<string[]> GetUserRoles(int userId);
        Task<bool> InsertUserRole(int userId, string role);
        Task<bool> DeleteUserRole(int userId, string role);
        Task<Document> DownloadDocument(int id);
        //Task<IEnumerable<Document>> DownloadUserDocuments(int userId);
        Task<int?> UploadDocument(Document document);
        //Task<Document> UpdateDocument(Document document);
        //Task DeleteDocument(int id);
        //Task DeleteDocuments(int[] ids);
        //Task CreateUserGroup(UserGroup group);
        //Task<UserGroup> GetUserGroup(int id);
        //Task<UserGroup> UpdateUserGroup(UserGroup group);
        //Task<bool> DeleteUserGroup(int id);
        //Task<bool> AddUserToGroup(int userId);
        //Task<bool> RemoveUserFromGroup(int userId);
    }
}
