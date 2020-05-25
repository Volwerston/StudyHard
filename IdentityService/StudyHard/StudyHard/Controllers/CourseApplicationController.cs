using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Domain;
using StudyHard.Helpers;
using StudyHard.Persistence.Interfaces;

namespace StudyHard.Controllers
{
    [Route("course")]
    public class CourseApplicationController : BaseController
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseApplicationRepository _courseApplicationRepository;

        public CourseApplicationController(
            ICourseRepository courseRepository,
            ICourseApplicationRepository courseApplicationRepository,
            IUserRepository userRepository,
            IUserInfoProvider userInfoProvider)
            : base(userRepository, userInfoProvider)
        {
            _courseRepository = courseRepository;
            _courseApplicationRepository = courseApplicationRepository;
        }

        public class CourseApplicationViewModel
        {
            public IReadOnlyCollection<CourseType> CourseTypes { get; set; }
        }

        [HttpGet("new")]
        public async Task<IActionResult> New()
        {
            var courseTypes = await _courseRepository.GetCourseTypes();

            return View(new CourseApplicationViewModel
            {
                CourseTypes = courseTypes
            });
        }

        public class CreateCourseApplicationRequest
        {
            [Required]
            public string Name { get; set; }

            [Required]
            public string ShortDescription { get; set; }

            public int CourseTypeId { get; set; }
        }

        public class CourseApplicationInfoViewModel
        {
            public CourseApplication Application { get; set; }
        }

        [HttpGet("info")]
        public async Task<IActionResult> Index([FromQuery] int courseApplicationId)
        {
            var courseApplication = await _courseApplicationRepository.Find(courseApplicationId);

            if (courseApplication == null)
            {
                return NotFound();
            }

            return View(new CourseApplicationInfoViewModel
            {
                Application = courseApplication
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCourseApplicationRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var courseTypes = await _courseRepository.GetCourseTypes();
            if (courseTypes.All(ct => ct.Id != request.CourseTypeId))
            {
                return BadRequest();
            }

            var courseApplication = new CourseApplication
            {
                Name = request.Name,
                ShortDescription = request.ShortDescription,
                Active = true,
                CourseType = courseTypes.Single(ct => ct.Id == request.CourseTypeId),
                CreatedDate = DateTime.UtcNow,
                UserId = (int) GetUserId()
            };

            var courseApplicationId = await _courseApplicationRepository.Create(courseApplication);

            return RedirectToAction("Index", new { courseApplicationId });
        }
    }
}