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
using StudyHard.Persistence.Interfaces;

namespace StudyHard.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourseApplicationRepository _courseApplicationRepository;
        private readonly ICourseRepository _courseRepository;
        public HomeController(IUserRepository userRepository, IUserInfoProvider userInfoProvider
            , ICourseApplicationRepository courseApplicationRepository, ICourseRepository courseRepository
            )
            : base(userRepository, userInfoProvider)
        {
            _userRepository = userRepository;
            _courseApplicationRepository = courseApplicationRepository;
            _courseRepository = courseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new HomeModel();

            if (UserInfoProvider.IsAuthenticated(User))
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
                    UserId = userId,
                    CourseApplications = await _courseApplicationRepository.GetCourseApplicationsForUser(userId),
                    CoursesAsCustomer = await _courseRepository.GetCoursesAsCustomer(userId),
                    CoursesAsTutor = await _courseRepository.GetCoursesAsTutor(userId),
                    PictureUrl = User?.Claims?.SingleOrDefault(x => x.Type == "Picture")?.Value ?? null
                };
            }
            
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateBio([FromForm] HomeModel form)
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
