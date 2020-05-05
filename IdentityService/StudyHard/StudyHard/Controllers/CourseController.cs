using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Models;
using StudyHard.Persistence.Interfaces;

namespace StudyHard.Controllers
{
    //for demonstration purposes only
    
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }
        [HttpGet]
        [Route("/course")]
        public async Task<ViewResult> Index()
        {
            //we may move it to some service, but if the logic is not complex - nothing bad to keep it in controller
            var courses = _courseRepository.GetCourses();
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
        
    }
}