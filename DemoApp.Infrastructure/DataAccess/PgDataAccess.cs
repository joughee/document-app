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

        #region Users
        public async Task<int?> CreateUser(User user)
        {
            int? userId = null;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT fn_user_insert(@email_address, @password, @active, @roles)";
                    cmd.Parameters.AddWithValue("email_address", user.EmailAddress);
                    cmd.Parameters.AddWithValue("password", user.PasswordHash);
                    cmd.Parameters.AddWithValue("active", user.Active);
                    cmd.Parameters.AddWithValue("roles", user.Roles);
                    userId = (int)(await cmd.ExecuteScalarAsync());
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return userId;
        }
        public async Task<User> GetUserById(int id)
        {
            User user = null;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT * FROM fn_user_get_by_id(@id)";
                    cmd.Parameters.AddWithValue("id", id);
                    var dr = await cmd.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        user = new User
                        {
                            Id = dr.GetInt32("Id"),
                            EmailAddress = dr.GetString("email_address"),
                            Active = dr.GetBoolean("active"),
                            PasswordHash = dr.GetString("password")
                        };
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return user;
        }
        public async Task<User> GetUserByUserName(string username)
        {
            User user = null;

            await using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT * FROM fn_user_get_by_name(@username)";
                    cmd.Parameters.AddWithValue("username", username);
                    await using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            user = new User
                            {
                                Id = dr.GetInt32("id"),
                                EmailAddress = dr.GetString("email_address"),
                                Active = dr.GetBoolean("active"),
                                PasswordHash = dr.GetString("password")
                            };
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return user;
        }
        public async Task<bool> UpdateUser(User user)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    if (!string.IsNullOrWhiteSpace(user.Password))
                    {
                        cmd.CommandText = "SELECT fn_user_update(@id, @email_address, @active, @password)";
                        cmd.Parameters.AddWithValue("password", user.PasswordHash);
                    }
                    else
                    {
                        cmd.CommandText = "SELECT fn_user_update(@id, @email_address, @active)";
                    }
                    cmd.Parameters.AddWithValue("id", user.Id);
                    cmd.Parameters.AddWithValue("email_address", user.EmailAddress);
                    cmd.Parameters.AddWithValue("active", user.Active);
                    success = await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return success;
        }
        public async Task<bool> DeleteUser(int id)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT fn_user_delete(@id)";
                    cmd.Parameters.AddWithValue("id", id);
                    success = await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return success;
        }
        #endregion

        #region Roles
        public async Task<string[]> GetUserRoles(int userId)
        {
            var roles = new List<string>();
            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT * FROM fn_user_roles_get(@user_id)";
                    cmd.Parameters.AddWithValue("user_id", userId);
                    var dr = await cmd.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        roles.Add(dr.GetString("role"));
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return roles.ToArray();
        }
        public async Task<bool> InsertUserRole(int id, string role)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT fn_user_role_insert(@id, @role)";
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("role", role);
                    success = await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return success;
        }
        public async Task<bool> DeleteUserRole(int id, string role)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT fn_user_role_delete(@id, @role)";
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("role", role);
                    success = await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return success;
        }
        #endregion

        #region GroupUsers
        public async Task<UserGroup[]> GetUsersGroups(int userId)
        {
            var groups = new List<UserGroup>();
            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT * FROM fn__user_user_groups_get(@user_id)";
                    cmd.Parameters.AddWithValue("user_id", userId);
                    var dr = await cmd.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        var group = new UserGroup
                        {
                            Id = dr.GetInt32("id"),
                            Name = dr.GetString("name"),
                            Active = dr.GetBoolean("active")
                        };
                        groups.Add(group);
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return groups.ToArray();
        }
        public async Task<UserGroup> GetUserGroup(int d)
        {
            var group = new UserGroup();
            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT * FROM fn_user_group_get(@id)";
                    cmd.Parameters.AddWithValue("id", d);
                    var dr = await cmd.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        group = new UserGroup
                        {
                            Id = dr.GetInt32("id"),
                            Name = dr.GetString("name"),
                            Active = dr.GetBoolean("active")
                        };
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return group;
        }
        public async Task<bool> InsertGroupUser(int groupId, int userId)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT fn_groupuser_insert(@groupId, @userId)";
                    cmd.Parameters.AddWithValue("groupId", groupId);
                    cmd.Parameters.AddWithValue("userId", userId);
                    success = await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return success;
        }
        #endregion

        #region Groups
        public async Task<bool> DeleteGroupUser(int userId, int groupId)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT fn_groupuser_delete(@userId, @groupId)";
                    cmd.Parameters.AddWithValue("userId", userId);
                    cmd.Parameters.AddWithValue("groupId", groupId);
                    success = await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return success;
        }
        public async Task<bool> InsertUserGroup(string name, bool active)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT fn_user_group_insert(@name, @active)";
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("active", active);
                    success = await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return success;
        }
        public async Task<bool> DeleteUserGroup(int id)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT fn_user_group_delete(@id)";
                    cmd.Parameters.AddWithValue("id", id);
                    success = await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return success;
        }
        public async Task<bool> UpdateUserGroup(UserGroup group)
        {
            var success = false;

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT fn_user_group_update(@id, @name, @active)";
                    cmd.Parameters.AddWithValue("id", group.Id);
                    cmd.Parameters.AddWithValue("name", group.Name);
                    cmd.Parameters.AddWithValue("active", group.Active);
                    success = await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return success;
        }
        #endregion

        #region Documents
        public async Task<int?> UploadDocument(Document document)
        {
            int? docId = null;
            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT fn_document_insert(@name, @description, @categories, @user_id, @date_time)";
                    cmd.Parameters.AddWithValue("name", document.Name);
                    cmd.Parameters.AddWithValue("description", document.Description);
                    cmd.Parameters.AddWithValue("categories", document.Categories);
                    cmd.Parameters.AddWithValue("user_id", document.InsertUserId);
                    cmd.Parameters.AddWithValue("date_time", document.InsertDateTime);
                    docId = (int)(await cmd.ExecuteScalarAsync());
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return docId;
        }

        public async Task<Document> DownloadDocument(int id)
        {
            var doc = new Document();

            using (var cmd = new NpgsqlCommand())
            {
                try
                {
                    _dbMainConnection.Open();
                    cmd.Connection = _dbMainConnection;
                    cmd.CommandText = "SELECT * FROM fn_document_get(@p1)";
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
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _dbMainConnection.Close();
                }
            }

            return doc;
        }
        #endregion

        //public async Task<IEnumerable<Document>> GetDocumentsByUserId(int userId)
        //{
        //    var result = new IdentityResult();

        //    using (var cmd = new NpgsqlCommand())
        //    {
        //        cmd.Connection = _dbConnection;
        //        cmd.CommandText = "SELECT * FROM my_stored_procedure(@p1, @p2)";
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
