using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Domain;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Domain;
using StudyHard.Helpers;
using StudyHard.Models;
using StudyHard.Persistence.Interfaces;

namespace StudyHard.Controllers
{
    //for demonstration purposes only

    [Authorize]
    public class CourseController : BaseController
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ITutorRepository _tutorRepository;

        public CourseController(
            ICourseRepository courseRepository,
            ITutorRepository tutorRepository,
            IUserRepository userRepository,
            IUserInfoProvider userInfoProvider)
            : base(userRepository, userInfoProvider)
        {
            _courseRepository = courseRepository;
            _tutorRepository = tutorRepository;
        }

        [HttpGet]
        [Route("/course")]
        public async Task<ViewResult> Index()
        {
            //we may move it to some service, but if the logic is not complex - nothing bad to keep it in controller
            CourseListModel model = new CourseListModel
            {
                Courses = await _courseRepository.GetCourses()
            };
            return View(model);
        }
        
        [Route("/course/{id}/api")]
        [HttpGet]
        public async Task<ActionResult> GetCourseInfo(int id)
        {
            //here we just display the data, so I didn't create a model
            var course = await _courseRepository.GetCourseById(id);
            return new JsonResult(course);
        }

        public class CourseViewModel
        {
            public long UserId { get; set; }
            public Course Course { get; set; }
            public Tutor Tutor { get; set; }
            public User Customer { get; set; }
            public CourseType CourseType { get; set; }
            public IReadOnlyCollection<CourseBlog> CourseBlogs { get; set; }
        }

        [HttpGet("/course/{id}/view")]
        public async Task<IActionResult> GetCourseView(int id)
        {
            var course = await _courseRepository.GetCourseById(id);
            if (course == null)
            {
                return NotFound();
            }

            var currentUserId = GetUserId();

            if (currentUserId != course.TutorId && currentUserId != course.CustomerId)
            {
                return NotFound();
            }

            var tutor = await _tutorRepository.Find(course.TutorId);
            var customer = UserRepository.FindUsers(new List<long>
            {
                course.CustomerId
            }).Single();

            var courseType = (await _courseRepository.GetCourseTypes())
                .Single(c => c.Id == course.CourseTypeId);

            var courseBlogs = await _courseRepository.GetCourseBlogs(id);

            return View(new CourseViewModel
            {
                UserId = GetUserId(),
                Course = course,
                Tutor = tutor,
                Customer = customer,
                CourseType = courseType,
                CourseBlogs = courseBlogs.OrderByDescending(cb => cb.CreationDateTimeUtc).ToArray()
            });
        }

        public class CreateBlogRequest
        {
            public int CourseId { get; set; }
            public int AuthorId { get; set; }

            [Required]
            public string BlogText { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlog([FromForm] CreateBlogRequest request)
        {
            var course = await _courseRepository.GetCourseById(request.CourseId);
            if (course == null)
            {
                return BadRequest();
            }

            var currentUserId = GetUserId();

            if (currentUserId != course.TutorId && currentUserId != course.CustomerId)
            {
                return BadRequest();
            }

            if (currentUserId != request.AuthorId)
            {
                return BadRequest();
            }

            var courseBlog = new CourseBlog
            {
                AuthorId = (int)currentUserId,
                CourseId = request.CourseId,
                CreationDateTimeUtc = DateTime.UtcNow,
                Text = request.BlogText
            };

            await _courseRepository.AddCourseBlog(courseBlog);

            return RedirectToAction("GetCourseView", new {id = request.CourseId});
        }
    }
}