using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IdentityService.Domain;
using IdentityService.Persistence.Interfaces;

namespace IdentityService.Persistence.Implementations
{
    public class UserRepository : IUserRepository
    {
        private IDbConnection _connection;
        private string _connectionString;

        public UserRepository(IDbConnection connection, string connectionString)
        {
            _connection = connection;
            _connectionString = connectionString;
        }

        public List<User> FindUsers(List<long> userIds)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<User>("SELECT * FROM [dbo].[User] WHERE Id IN @UserIds",
                    new {UserIds = userIds}).ToList();
            }
        }

        public async Task<IReadOnlyCollection<Role>> FindRoles(User user)
            => (await _connection.QueryAsync<Role>(
                @"SELECT R.Id, R.Name 
                     FROM[dbo].[User] U
                     INNER JOIN[dbo].[UserRoles] UR ON U.Id = UR.UserId
                     INNER JOIN[dbo].[Role] R ON R.Id = UR.RoleId
                     WHERE U.Email = @Email",
                new {user.Email})).ToArray();

        public Task Save(User user)
        {
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@email", user.Email);
            queryParameters.Add("@name", user.Name);
            queryParameters.Add("@pictureUrl", user.PictureUrl);

            var command = new CommandDefinition(
                "GetOrAddUser",
                queryParameters,
                commandType: CommandType.StoredProcedure);

            return _connection.ExecuteAsync(command);
        }
        public Task Update(User user)
        {
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@UserId", user.Id);
            queryParameters.Add("@Name", user.Name);
            queryParameters.Add("@Gender", (int) user.Gender); 
            queryParameters.Add("@BirthDate", user.BirthDate);

            var command = new CommandDefinition(
                "UpdateUser",
                queryParameters,
                commandType: CommandType.StoredProcedure);

            return _connection.ExecuteAsync(command);
        }

        public long GetUserIdByEmail(string email)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var users = db.Query<User>("SELECT * FROM [dbo].[User] WHERE Email = @Email",
                    new {Email = email}).ToList();

                if (users.Any())
                {
                    return users.First().Id;
                }

                return -1;
            }
        }
    }
}
