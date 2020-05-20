using System.Linq;
using System.Security.Claims;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace StudyHard.Controllers
{
    public class BaseController: Controller
    {
        private readonly IUserRepository _userRepository;
        public BaseController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public long GetUserId() => _userRepository.GetUserIdByEmail(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email)?.Value);
    }
}