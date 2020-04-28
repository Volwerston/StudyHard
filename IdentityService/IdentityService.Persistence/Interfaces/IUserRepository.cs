using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Domain;

namespace IdentityService.Persistence.Interfaces
{
    public interface IUserRepository
    {
        Task<IReadOnlyCollection<Role>> FindRoles(User user);
        Task Save(User user);
    }
}
