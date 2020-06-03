using System.Collections.Generic;
using System.Threading.Tasks;
using StudyHard.Domain;

namespace StudyHard.Persistence.Interfaces
{
    public interface ICourseApplicationRepository
    {
        Task<CourseApplication> Find(int applicationId);
        List<CourseApplication> Find(string name, List<int> courseTypes);
        Task<int> Create(CourseApplication application);
        Task Deactivate(int courseApplicationId);
        Task<List<CourseApplication>> GetCourseApplicationsForUser(long userId);
    }
}
