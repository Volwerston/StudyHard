using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Domain;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudyHard.Helpers;
using StudyHard.Models;

namespace StudyHard.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, IUserInfoProvider userInfoProvider)
            : base(userRepository, userInfoProvider)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new HomeModel();

            if (User.Identity.IsAuthenticated)
            {
                long userId = GetUserId();

                var user = _userRepository.FindUsers(new List<long> { userId }).Single(); 

                var roles = (await _userRepository.FindRoles(user)).ToList();
                
                model = new HomeModel
                {
                    BirthDate = user.BirthDate,
                    Name = user.Name,
                    Email = user.Email,
                    Gender = user.Gender,
                    Roles = roles,
                    UserId = userId
                };
            }
            
            return View(model);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] HomeModel form)
        {
            if (ModelState.IsValid)
            {
                //Better to use dto for this
                var user = new User
                {
                    Id = form.UserId,
                    Name = form.Name,
                    Gender = form.Gender ?? 0,
                    BirthDate = form.BirthDate
                };
                await _userRepository.Update(user);
                    
            }
            return RedirectPermanent("/Home/Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
