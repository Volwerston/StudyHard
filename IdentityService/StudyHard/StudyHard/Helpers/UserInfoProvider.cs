using System.Linq;
using System.Security.Claims;

namespace StudyHard.Helpers
{
    public interface IUserInfoProvider
    {
        string GetUserEmail(ClaimsPrincipal claimsPrincipal);
        bool IsAuthenticated(ClaimsPrincipal claimsPrincipal);
    }

    public class UserInfoProvider : IUserInfoProvider
    {
        public string GetUserEmail(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims
                .SingleOrDefault(c => c.Type == ClaimTypes.Email)?
                .Value;
        }
        public string GetUserPictureUrl(ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.Claims.SingleOrDefault(x => x.Type == "Picture")?.Value;
        public bool IsAuthenticated(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Identity.IsAuthenticated;
        }
    }
}
