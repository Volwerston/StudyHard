using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Domain;

namespace IdentityService.Persistence.Interfaces
{
    public interface IUserRepository
    {
        List<User> FindUsers(List<long> userIds);
        Task<IReadOnlyCollection<Role>> FindRoles(User user);
        Task Save(User user);
        Task Update(User user);

        long GetUserIdByEmail(string email);
    }
}
