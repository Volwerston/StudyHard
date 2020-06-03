using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Domain;
using StudyHard.Helpers;
using StudyHard.Models;
using StudyHard.Persistence.Interfaces;

namespace StudyHard.Controllers
{
    [Authorize]
    [Route("course")]
    public class CourseApplicationController : BaseController
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly ICourseApplicationRepository _courseApplicationRepository;

        public CourseApplicationController(
            ICourseRepository courseRepository,
            ICourseApplicationRepository courseApplicationRepository,
            ITutorRepository tutorRepository,
            IUserRepository userRepository,
            IUserInfoProvider userInfoProvider)
            : base(userRepository, userInfoProvider)
        {
            _courseRepository = courseRepository;
            _tutorRepository = tutorRepository;
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
            [Required] public string Name { get; set; }

            [Required] public string ShortDescription { get; set; }

            public int CourseTypeId { get; set; }
        }

        public class CourseApplicationInfoViewModel
        {
            public long UserId { get; set; }
            public bool CanAcceptCourse { get; set; }
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

            var canAcceptCourse = false;
            var currentUserId = GetUserId();

            if (courseApplication.Active)
            {
                var tutor = await _tutorRepository.Find((int) currentUserId);

                if (tutor != null && tutor.Id != courseApplication.UserId)
                {
                    var courses = await _tutorRepository.GetCourses(tutor.Id);
                    canAcceptCourse = courses.Any(course => course.Id == courseApplication.CourseType.Id);
                }
            }

            return View(new CourseApplicationInfoViewModel
            {
                UserId = currentUserId,
                CanAcceptCourse = canAcceptCourse,
                Application = courseApplication
            });
        }

        public class AcceptCourseApplicationRequest
        {
            public int TutorId { get; set; }

            public int CourseApplicationId { get; set; }
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptApplication([FromForm] AcceptCourseApplicationRequest request)
        {
            var courseApplication = await _courseApplicationRepository.Find(request.CourseApplicationId);
            if (courseApplication == null)
            {
                return BadRequest();
            }

            var tutor = await _tutorRepository.Find(request.TutorId);
            if (tutor == null)
            {
                return BadRequest();
            }

            var currentUserId = GetUserId();
            if (currentUserId != tutor.Id)
            {
                return BadRequest();
            }

            if (request.TutorId == courseApplication.UserId)
            {
                return BadRequest();
            }

            var courses = await _tutorRepository.GetCourses(tutor.Id);
            if (courses.All(c => c.Id != courseApplication.CourseType.Id))
            {
                return BadRequest();
            }

            var course = new Course
            {
                Name = courseApplication.Name,
                Description = courseApplication.ShortDescription,
                CourseTypeId = courseApplication.CourseType.Id,
                CreatedDate = DateTime.UtcNow,
                Active = true,
                CourseApplicationId = courseApplication.Id,
                CustomerId = courseApplication.UserId,
                TutorId = request.TutorId
            };

            var courseId = await _courseRepository.CreateCourse(course);
            await _courseApplicationRepository.Deactivate(courseApplication.Id);

            return RedirectToAction("GetCourseView", "Course", new {id = courseId});
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

            return RedirectToAction("Index", new {courseApplicationId});
        }

        [HttpGet("infos")]
        public IActionResult GetCourses([FromQuery] string name, [FromQuery] List<int> courseTypes,
            [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            return Ok(_courseApplicationRepository.Search(GetUserId(), name, courseTypes)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize));
        }

        [Authorize]
        [HttpGet("search")]
        public ViewResult Search()
        {
            var courseTypes = _courseRepository.GetCourseTypes().Result;

            return View(new CASearchViewModel
            {
                Skills = courseTypes.ToArray()
            });
        }
    }
}