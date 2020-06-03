using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Helpers;

namespace StudyHard.Controllers
{
    public class BaseController: Controller
    {
        protected readonly IUserRepository UserRepository;
        protected readonly IUserInfoProvider UserInfoProvider;

        public BaseController(IUserRepository userRepository, IUserInfoProvider userInfoProvider)
        {
            UserRepository = userRepository;
            UserInfoProvider = userInfoProvider;
        }

        public long GetUserId() => UserRepository.GetUserIdByEmail(
            UserInfoProvider.GetUserEmail(User));
    }
}