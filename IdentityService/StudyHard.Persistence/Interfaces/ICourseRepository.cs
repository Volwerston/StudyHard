using StudyHard.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StudyHard.Persistence.Interfaces
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetCourses();
        Task<Course> GetCourseById(int id);
        Task<IReadOnlyCollection<CourseType>> GetCourseTypes();
        Task<int> CreateCourse(Course course);
    }
}
