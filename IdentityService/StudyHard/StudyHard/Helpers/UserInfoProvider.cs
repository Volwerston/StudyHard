using System.Linq;
using System.Security.Claims;

namespace StudyHard.Helpers
{
    public interface IUserInfoProvider
    {
        string GetUserEmail(ClaimsPrincipal claimsPrincipal);
    }

    public class UserInfoProvider : IUserInfoProvider
    {
        public string GetUserEmail(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims
                .SingleOrDefault(c => c.Type == ClaimTypes.Email)?
                .Value;
        }
    }
}
