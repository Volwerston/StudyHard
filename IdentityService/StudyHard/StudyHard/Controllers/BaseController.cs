using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Helpers;

namespace StudyHard.Controllers
{
    public class BaseController: Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserInfoProvider _userInfoProvider;

        public BaseController(IUserRepository userRepository, IUserInfoProvider userInfoProvider)
        {
            _userRepository = userRepository;
            _userInfoProvider = userInfoProvider;
        }

        public long GetUserId() => _userRepository.GetUserIdByEmail(
            _userInfoProvider.GetUserEmail(User));
    }
}