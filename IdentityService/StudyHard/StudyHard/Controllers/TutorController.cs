using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Domain;
using StudyHard.Models;
using StudyHard.Persistence.Interfaces;

namespace StudyHard.Controllers
{
    [Route("tutors")]
    public class TutorController : Controller
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly ICourseRepository _courseRepository;

        public TutorController(
            ITutorRepository tutorRepository,
            ICourseRepository courseRepository)
        {
            _tutorRepository = tutorRepository;
            _courseRepository = courseRepository;
        }

        public class FindTutorsRequest
        {
            [Required]
            public int PageNumber { get; set; }

            [Required]
            public int PageSize { get; set; }

            public string[] Courses { get; set; }
        }

        [HttpPost("find")]
        public async Task<IActionResult> Find([FromBody] FindTutorsRequest request)
        {
            if (request.Courses == null || request.Courses.Length == 0)
            {
                var courseTypes = await _courseRepository.GetCourseTypes();

                request.Courses = courseTypes.Select(c => c.Type).ToArray();
            }

            var tutors = await _tutorRepository.Find(
                request.Courses,
                request.PageNumber,
                request.PageSize);

            return Ok(tutors);
        }

        public class TutorPersonalPageViewModel
        {
            public Tutor Tutor { get; set; }
            public IReadOnlyCollection<CourseType> Courses { get; set; }
            public IReadOnlyCollection<Blog> Blogs { get; set; }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> PersonalPage(int tutorId)
        {
            var tutor = await _tutorRepository.Find(tutorId);
            if (tutor == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var courses = await _tutorRepository.GetCourses(tutorId);
            var blogs = await _tutorRepository.GetBlogs(tutorId);

            return View(new TutorPersonalPageViewModel
            {
                Tutor = tutor,
                Courses = courses,
                Blogs = blogs
            });
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ViewResult> Search()
        {
            var courseTypes = await _courseRepository.GetCourseTypes();

            return View(new TutorSearchViewModel
            {
                Skills = courseTypes.Select(ct => ct.Type).ToArray()
            });
        }
    }
}