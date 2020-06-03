using StudyHard.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyHard.Persistence.Interfaces
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetCourses();
        Task<List<Course>> GetCoursesAsTutor(long userId);
        Task<List<Course>> GetCoursesAsCustomer(long userId);
        Task<Course> GetCourseById(int id);
        Task<IReadOnlyCollection<CourseType>> GetCourseTypes();
        Task<int> CreateCourse(Course course);
        Task AddCourseBlog(CourseBlog blog);
        Task<IReadOnlyCollection<CourseBlog>> GetCourseBlogs(int courseId);
    }
}
