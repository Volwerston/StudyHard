using System.Collections.Generic;
using System.Data;
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

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IReadOnlyCollection<Role>> FindRoles(User user)
            => (await _connection.QueryAsync<Role>(
                "SELECT Role as Name FROM [dbo].[user_role] WHERE Email = @Email",
                new {user.Email})).ToArray();

        public Task Save(User user)
        {
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@email", user.Email);
            queryParameters.Add("@role", user.Role);

            var command = new CommandDefinition(
                "GetOrAddUser",
                queryParameters,
                commandType: CommandType.StoredProcedure);

            return _connection.ExecuteAsync(command);
        }
    }
}
