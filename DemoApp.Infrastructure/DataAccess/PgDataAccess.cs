using DemoApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using DemoApp.Infrastructure.Configuration;
using System.Linq;

namespace DemoApp.Infrastructure.DataAccess
{
    public class PgDataAccess : IDataAccessManager
    {
        private DataAccessSettings _configSettings;
        private NpgsqlConnection _dbMainConnection { get; }
        private NpgsqlConnection _dbFileConnection { get; set; }

        //private NpgsqlTransaction _dbTransaction { get; set; }
        //public DataAccessManager(IDbConnection connection, IDbTransaction transaction)
        //{
        //    _dbConnection = new NpgsqlConnection(connection.ConnectionString);
        //    _dbTransaction = new Npgsql.NpgsqlTransaction();
        //}
        public PgDataAccess(DataAccessSettings configSettings)
        {
            _configSettings = configSettings;
            _dbMainConnection = new NpgsqlConnection(configSettings.MainDbConnectionString);
            //_dbFileConnection = new NpgsqlConnection(configSettings.FileDbConnection);
            //_dbTransaction = null;
        }

        //public async Task<IdentityResult> CreateUserAsync(User user)
        //{
        //    var result = new IdentityResult(_dbMainConnection);

        //    using (var cmd = new NpgsqlCommand())
        //    {
        //        cmd.Connection = _dbConnection;
        //        cmd.CommandText = "CALL my_stored_procedure(@p1, @p2)";
        //        cmd.Parameters.AddWithValue("p1", "");
        //        cmd.Parameters.AddWithValue("p2", "");
        //        if (await cmd.ExecuteNonQueryAsync() > 0)
        //        {
        //            result = IdentityResult.Success;
        //        }
        //        else
        //        {
        //            result = IdentityResult.Failed(new IdentityError { Description = $"Error occerred, unable to create user {user.EmailAddress}" });
        //        }
        //    }

        //    return result;
        //}
        public async Task<int> CreateUser(User user)
        {
            int userId;

            using (var cmd = new NpgsqlCommand())
            {
                _dbMainConnection.Open();
                cmd.Connection = _dbMainConnection;
                cmd.CommandText = "CALL sp_user_insert(@email_address, @password, @active, @roles)";
                cmd.Parameters.AddWithValue("email_address", user.EmailAddress);
                cmd.Parameters.AddWithValue("password", user.PasswordHash);
                cmd.Parameters.AddWithValue("active", user.Active);
                cmd.Parameters.AddWithValue("roles", user.Roles);
                userId = (int)(await cmd.ExecuteScalarAsync());
                //if (userId > 0 && user.Roles != null && user.Roles.Any())
                //{
                //    foreach(var r in user.Roles)
                //    {
                //        cmd.Parameters.Clear();
                //        cmd.CommandText = "CALL sp_user_role_insert(@user_id, @role)";
                //        cmd.Parameters.AddWithValue("user_id", userId);
                //        cmd.Parameters.AddWithValue("role", user.PasswordHash);
                //        await cmd.ExecuteScalarAsync();
                //    }
                //}
                _dbMainConnection.Close();
            }

            return userId;
        }
        public async Task<User> GetUserById(int id)
        {
            User user = null;

            using (var cmd = new NpgsqlCommand())
            {
                _dbMainConnection.Open();
                cmd.Connection = _dbMainConnection;
                cmd.CommandText = "CALL sp_user_get_by_id(@id)";
                cmd.Parameters.AddWithValue("id", id);
                var dr = await cmd.ExecuteReaderAsync();
                while (dr.Read())
                {
                    user = new User
                    {
                        Id = dr.GetInt32("Id"),
                        EmailAddress = dr.GetString("email_address"),
                        PasswordHash = dr.GetString("password"),
                        Active = dr.GetBoolean("active")
                    };
                }
                _dbMainConnection.Close();
            }

            return user;
        }
        public async Task<User> GetUserByUserName(string username)
        {
            User user = null;

            using (var cmd = new NpgsqlCommand())
            {
                _dbMainConnection.Open();
                cmd.Connection = _dbMainConnection;
                cmd.CommandText = "CALL sp_user_get_by_name(@username)";
                cmd.Parameters.AddWithValue("username", username);
                var dr = await cmd.ExecuteReaderAsync();
                while (dr.Read())
                {
                    user = new User
                    {
                        Id = dr.GetInt32("Id"),
                        EmailAddress = dr.GetString("email_address"),
                        PasswordHash = dr.GetString("password"),
                        Active = dr.GetBoolean("active")
                    };
                }
                _dbMainConnection.Close();
            }

            return user;
        }
        public async Task<bool> UpdateUser(User user)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                _dbMainConnection.Open();
                cmd.Connection = _dbMainConnection;
                cmd.CommandText = "CALL sp_user_update(@id, @email_address, @password, @active)";
                cmd.Parameters.AddWithValue("id", user.Id);
                cmd.Parameters.AddWithValue("email_address", user.EmailAddress);
                cmd.Parameters.AddWithValue("password", user.PasswordHash);
                cmd.Parameters.AddWithValue("active", user.Active);
                success = await cmd.ExecuteNonQueryAsync() > 0;
                _dbMainConnection.Close();
            }

            return success;
        }
        public async Task<bool> DeleteUser(int id)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                _dbMainConnection.Open();
                cmd.Connection = _dbMainConnection;
                cmd.CommandText = "CALL sp_user_delete(@id)";
                cmd.Parameters.AddWithValue("id", id);
                success = await cmd.ExecuteNonQueryAsync() > 0;
                _dbMainConnection.Close();
            }

            return success;
        }
        public async Task<string[]> GetUserRoles(int userId)
        {
            var roles = new List<string>();
            using (var cmd = new NpgsqlCommand())
            {
                _dbMainConnection.Open();
                cmd.Connection = _dbMainConnection;
                cmd.CommandText = "CALL sp_user_roles_get(@user_id)";
                cmd.Parameters.AddWithValue("user_id", userId);
                var dr = await cmd.ExecuteReaderAsync();
                while (dr.Read())
                {
                    roles.Add(dr.GetString("role"));
                }
                _dbMainConnection.Close();
            }

            return roles.ToArray();
        }
        public async Task<bool> InsertUserRole(int id, string role)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                _dbMainConnection.Open();
                cmd.Connection = _dbMainConnection;
                cmd.CommandText = "CALL sp_user_role_insert(@id, @role)";
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("role", role);
                success = await cmd.ExecuteNonQueryAsync() > 0;
                _dbMainConnection.Close();
            }

            return success;
        }
        public async Task<bool> DeleteUserRole(int id, string role)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                _dbMainConnection.Open();
                cmd.Connection = _dbMainConnection;
                cmd.CommandText = "CALL sp_user_role_delete(@id, @role)";
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("role", role);
                success = await cmd.ExecuteNonQueryAsync() > 0;
                _dbMainConnection.Close();
            }

            return success;
        }

        public async Task<int> UploadDocument(Document document)
        {
            int docId;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = _dbMainConnection;
                cmd.CommandText = "CALL sp_document_insert(@name, @description, @categories, @user_id, @date_time)";
                cmd.Parameters.AddWithValue("name", document.Name);
                cmd.Parameters.AddWithValue("description", document.Description);
                cmd.Parameters.AddWithValue("categories", document.Categories);
                cmd.Parameters.AddWithValue("user_id", document.InsertUserId);
                cmd.Parameters.AddWithValue("date_time", document.InsertDateTime);
                docId = (int)(await cmd.ExecuteScalarAsync());

            }

            return docId;
        }

        public async Task<Document> DownloadDocument(int id)
        {
            var doc = new Document();

            using (var cmd = new NpgsqlCommand())
            {
                _dbMainConnection.Open();
                cmd.Connection = _dbMainConnection;
                cmd.CommandText = "CALL sp_document_get(@p1)";
                cmd.Parameters.AddWithValue("p1", id);
                var dr = await cmd.ExecuteReaderAsync();
                while (dr.Read())
                {
                    doc = new Document 
                    {
                        Id = dr.GetInt32("Id"),
                        Name = dr.GetString("Name"),
                        Description = dr.GetString("Description"),
                        Categories = (string[])dr["Categories"],
                        InsertUserId = dr.GetInt32("InsertUserId"),
                        InsertDateTime = dr.GetDateTime("InsertDateTime"),
                        ModifyUserId = dr.GetInt32("ModifyUserId"),
                        ModifyDateTime = dr.GetDateTime("ModifyDateTime")
                    };
                }
                _dbMainConnection.Close();
            }

            return doc;
        }

        //public async Task<IEnumerable<Document>> GetDocumentsByUserId(int userId)
        //{
        //    var result = new IdentityResult();

        //    using (var cmd = new NpgsqlCommand())
        //    {
        //        cmd.Connection = _dbConnection;
        //        cmd.CommandText = "CALL my_stored_procedure(@p1, @p2)";
        //        cmd.Parameters.AddWithValue("p1", "");
        //        cmd.Parameters.AddWithValue("p2", "");
        //        if (await cmd.ExecuteNonQueryAsync() > 0)
        //        {
        //            result = IdentityResult.Success;
        //        }
        //        else
        //        {
        //            result = IdentityResult.Failed(new IdentityError { Description = $"Error occerred, unable to create user {user.Email}" });
        //        }
        //    }

        //    return result;
        //}
        //public async Task CreateUserGroup(UserGroup group)
        //{

        //}

        //public async Task<UserGroup> GetUserGroup(int id)
        //{

        //}

        //public async Task<UserGroup> UpdateUserGroup(UserGroup group)
        //{

        //}

        //public async Task<bool> DeleteUserGroup(int id)
        //{

        //}

        //public async Task<bool> AddUserToGroup(int userId)
        //{

        //}

        //public async Task<bool> RemoveUserFromGroup(int userId)
        //{

        //}
    }
}
