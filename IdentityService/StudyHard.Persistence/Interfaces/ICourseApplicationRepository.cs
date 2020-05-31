using System.Threading.Tasks;
using StudyHard.Domain;

namespace StudyHard.Persistence.Interfaces
{
    public interface ICourseApplicationRepository
    {
        Task<CourseApplication> Find(int applicationId);
        Task<int> Create(CourseApplication application);

        Task Deactivate(int courseApplicationId);
    }
}
