using StudyHard.Domain;
using StudyHard.Persistence.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using StudyHard.Persistence.Dtos;

namespace StudyHard.Persistence.Implementations
{
    public class TutorRepository : ITutorRepository
    {
        private readonly string _connectionString;

        public TutorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddBlog(int tutorId, Blog blog)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(
                    @"INSERT INTO [dbo].[Blog] VALUES(@Title, @Text, @CreationDateTimeUtc, @tutorId)",
                    new { blog.Title, blog.Text, blog.CreationDateTimeUtc, tutorId });
            }
        }

        public async Task<IReadOnlyCollection<Tutor>> Find(string[] courses, int pageNumber, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                      SELECT DISTINCT U.Id, U.Email, U.Name, U.PictureUrl
                        FROM [dbo].[User] U 
                        INNER JOIN [dbo].[UserRoles] UR ON U.Id = UR.UserId
                        INNER JOIN [dbo].[Role] R ON R.Id = UR.RoleId
                        INNER JOIN [dbo].[UserSkills] US ON U.Id = US.UserId
                        INNER JOIN [dbo].[CourseType] CT ON US.SkillId = CT.Id
                        WHERE R.Name='Tutor' AND CT.Type IN @courses
                        ORDER BY U.Id 
                        OFFSET @offsetCount ROWS FETCH NEXT @fetchCount ROWS ONLY
                        
                      SELECT U.Id as UserId, CT.Id as CourseId, CT.Type
                        FROM [dbo].[User] U 
                        INNER JOIN [dbo].[UserRoles] UR ON U.Id = UR.UserId
                        INNER JOIN [dbo].[Role] R ON R.Id = UR.RoleId
                        INNER JOIN [dbo].[UserSkills] US ON U.Id = US.UserId
                        INNER JOIN [dbo].[CourseType] CT ON US.SkillId = CT.Id
                        WHERE R.Name='Tutor'";

                using (var multi = await connection.QueryMultipleAsync(
                    sql,
                    new
                    {
                        courses,
                        offsetCount = (pageNumber - 1) * pageSize,
                        fetchCount = pageSize
                    }))
                {
                    var tutors = multi.Read<UserDto>().ToDictionary(t => t.Id, t => new Tutor
                    {
                        Email = t.Email,
                        Id = t.Id,
                        Name = t.Name,
                        PictureUrl = t.PictureUrl,
                        Skills = new List<CourseType>()
                    });

                    var courseTypes = multi.Read<CourseTypeDto>().ToList();

                    foreach (var courseType in courseTypes)
                    {
                        if (tutors.ContainsKey(courseType.UserId))
                        {
                            tutors[courseType.UserId].Skills.Add(new CourseType
                            {
                                Id = courseType.CourseId,
                                Type = courseType.Type
                            });
                        }
                    }

                    return tutors.Select(t => t.Value).ToArray();
                }
            }
        }

        public async Task<Tutor> Find(int tutorId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QuerySingleOrDefaultAsync<Tutor>(
                    @"SELECT U.Id, U.Name, U.Email, U.PictureUrl
                         FROM [dbo].[User] U
                         INNER JOIN [dbo].[UserRoles] UR ON U.Id = UR.UserId
                         INNER JOIN [dbo].[Role] R ON R.Id = UR.RoleId
                         WHERE U.Id = @tutorId AND R.Name='Tutor'",
                    new { tutorId });
            }
        }

        public async Task<IReadOnlyCollection<Blog>> GetBlogs(int tutorId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return (await connection.QueryAsync<Blog>(
                    @"SELECT * FROM [dbo].[Blog] WHERE UserId = @tutorId",
                    new { tutorId })).ToArray();
            }
        }

        public async Task<IReadOnlyCollection<CourseType>> GetCourses(int tutorId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return (await connection.QueryAsync<CourseType>(
                    @"SELECT CT.Id, CT.Type FROM [dbo].[User] U
                INNER JOIN [dbo].[UserSkills] US ON U.Id = US.UserId
                INNER JOIN [dbo].[CourseType] CT ON CT.Id = US.SkillId
                WHERE U.Id = @tutorId",
                    new { tutorId })).ToArray();
            }
        }
    }
}