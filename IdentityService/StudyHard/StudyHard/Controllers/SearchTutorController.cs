using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Persistence.Interfaces;

namespace StudyHard.Controllers
{
    [Route("tutors")]
    public class SearchTutorController : Controller
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly ICourseRepository _courseRepository;

        public SearchTutorController(
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
        public async Task<IActionResult> Index([FromBody] FindTutorsRequest request)
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
    }
}