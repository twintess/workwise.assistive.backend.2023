using db.mrds.net;
using utils.mrds.net;
using Microsoft.Data.SqlClient;
using System.Data;
using workwise.assistive.backend.Model;
using BCrypt.Net;
using Azure.Core;
using System.Transactions;
using auth.mrds.net;
using System.Data.Common;

namespace workwise.assistive.backend.Service
{
    public class UserService
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        private readonly string _connectionString;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;            
            _connectionString = _configuration.GetConnectionString("DB");
        }

        public AuthenticationResponse? Authenticate(AuthenticationRequest authenticationRequest)
        {
            try
            {
                var isHashCorrect = BcryptProvider.VerifyPassword(authenticationRequest.Password,
                    GetAuthenticationDetails(authenticationRequest.Username));

                if (isHashCorrect)
                {
                    AuthenticationResponse result = null;
                    result = new AuthenticationResponse
                    {
                        Username = authenticationRequest.Username,
                        Roles = GetRoles(authenticationRequest.Username)
                    };

                    return result;
                }
                else
                {
                    _logger.LogError("Authentication failed");
                    return null;
                }
            }
            catch(Exception exc)
            {
                _logger.LogError(exc.ToString());
                return null;
            }
        }

        private string GetAuthenticationDetails(string username)
        {
            try
            {
                SqlConnection connection;
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = new SqlCommand("GetAuthenticationDetails", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Parameters = { new SqlParameter("username", username) }
                    };

                    var dbm = new DBMasterClass(new DatabaseParameter(connection.ConnectionString, ConnectionType.NET));
                    return dbm.SelectOneValue(query);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                return string.Empty;
            }
        }

        public List<string>? GetRoles(string username)
        {
            var result = new List<string>();

            try
            {
                DataTable? rolesTable;

                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = new SqlCommand("GetRolesForUser", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Parameters =
                        {
                            new SqlParameter("username", username)
                        }
                    };

                    var dbm = new DBMasterClass(new DatabaseParameter(connection.ConnectionString, ConnectionType.NET));
                    rolesTable = dbm.GetDataTable(query);
                }


                if(rolesTable != null )
                {
                    foreach(DataRow row in rolesTable.Rows)
                    {
                        result.Add(DbUtils.CheckString(row.Field<string>("role_name")));
                    }
                }

                return result;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                return result;
            }
        }

        public bool CreateUser(NewUserRequest request)
        {
            SqlConnection connection = null;
            try
            {
                request.Password = BcryptProvider.HashPassword(request.Password);

                SqlTransaction transaction;

                int userId;

                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    var query = new SqlCommand("CreateUser", connection, transaction)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Parameters = {
                            new SqlParameter("username", request.Username),
                            new SqlParameter("password", request.Password),
                            new SqlParameter("firstname", request.Firstname),
                            new SqlParameter("lastname", request.Lastname),
                            new SqlParameter("email", request.Email),
                        }
                    };

                    var dbm = new DBMasterClass(new DatabaseParameter(connection.ConnectionString, ConnectionType.NET));
                    userId = int.Parse(dbm.InserUpdateOneValueInTransaction(query));

                    _logger.LogInformation($"User with id {userId} created succesfully");

                    if(request.Roles != null)
                    {
                        var result = AssignRolesForUser(connection, transaction, userId, request.Roles);

                        if (result)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            _logger.LogError("Assigning roles failed. Cancell user creation");
                            transaction.Rollback();
                        }
                    }
                    else
                    {
                        _logger.LogError("Roles are not defined for user. Cancell user creation");
                        transaction.Rollback();
                    }
                }

                return true;
            }
            catch(Exception exc)
            {
                _logger.LogError("User not created");
                _logger.LogError(exc.ToString());
                return false;
            }
            finally
            {
                connection?.Close();
            }
        }

        private bool AssignRolesForUser(SqlConnection connection, SqlTransaction transaction, int userId, List<string> roles)
        {
            try
            {
                foreach (var role in roles)
                {
                    var query = new SqlCommand("AssignRoleToUser", connection, transaction)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Parameters = {
                            new SqlParameter("userId", userId),
                            new SqlParameter("role", role)
                        }
                    };

                    var dbm = new DBMasterClass(new DatabaseParameter(connection.ConnectionString, ConnectionType.NET));
                    dbm.InserUpdateOneValueInTransaction(query);

                    _logger.LogInformation($"Role {role} assigned to userid {userId}");
                }

                return true;
            }
            catch(Exception exc)
            {
                _logger.LogError(exc.ToString());
                return false;
            }
        }
    }
}
