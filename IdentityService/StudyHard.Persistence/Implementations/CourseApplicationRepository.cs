using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using StudyHard.Domain;
using StudyHard.Persistence.Interfaces;

namespace StudyHard.Persistence.Implementations
{
    public class CourseApplicationRepository : ICourseApplicationRepository
    {
        private readonly string _connectionString;

        public CourseApplicationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CourseApplication> Find(int applicationId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return (await connection.QueryAsync<CourseApplication, CourseType, CourseApplication>(
                        @"SELECT 
                        	CA.Id, 
                        	CA.Name, 
                        	CA.ShortDescription, 
                        	CA.ApplicantId as UserId, 
                        	CA.CreatedDate, 
                        	CA.Active,
                        	CT.Id,
                        	CT.Type
                        FROM [dbo].[CourseApplication] CA 
                        INNER JOIN [dbo].[CourseType] CT ON CA.CourseTypeId = CT.Id
                        WHERE CA.Id=@applicationId",
                        (ca, ct) =>
                        {
                            ca.CourseType = ct;
                            return ca;
                        },
                        new { applicationId },
                        splitOn: "Id"
                    ))
                    .SingleOrDefault();
            }
        }

        public async Task<int> Create(CourseApplication application)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QuerySingleAsync<int>(
                    @"INSERT INTO [dbo].[CourseApplication]
                        (Name, ShortDescription, ApplicantId, CourseTypeId, CreatedDate, Active)
                        VALUES (@Name, @ShortDescription, @UserId, @CourseTypeId, @CreatedDate, @Active);
                        SELECT IDENT_CURRENT('dbo.CourseApplication');",
                    new
                    {
                        application.Name,
                        application.ShortDescription,
                        application.UserId,
                        CourseTypeId = application.CourseType.Id,
                        application.CreatedDate,
                        application.Active
                    });
            }
        }
    }
}
