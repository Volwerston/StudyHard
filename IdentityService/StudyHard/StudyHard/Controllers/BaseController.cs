using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Helpers;

namespace StudyHard.Controllers
{
    public class BaseController: Controller
    {
        protected readonly IUserRepository UserRepository;
        private readonly IUserInfoProvider _userInfoProvider;

        public BaseController(IUserRepository userRepository, IUserInfoProvider userInfoProvider)
        {
            UserRepository = userRepository;
            _userInfoProvider = userInfoProvider;
        }

        public long GetUserId() => UserRepository.GetUserIdByEmail(
            _userInfoProvider.GetUserEmail(User));
    }
}