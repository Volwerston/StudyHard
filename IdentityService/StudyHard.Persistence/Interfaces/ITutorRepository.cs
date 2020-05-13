using System.Collections.Generic;
using System.Threading.Tasks;
using StudyHard.Domain;

namespace StudyHard.Persistence.Interfaces
{
    public interface ITutorRepository
    {
        Task<IReadOnlyCollection<Tutor>> Find(string[] courses, int pageNumber, int pageSize);
    }
}
