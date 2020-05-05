using Dapper;
using StudyHard.Domain;
using StudyHard.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHard.Persistence.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly string _connectionString;
        public CourseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<List<Course>> GetCourses()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return (await db.QueryAsync<Course>("SELECT * FROM Course")).ToList();
            }
        }

        public async Task<Course> GetCourseById(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<Course>("SELECT * FROM Course WHERE Id = @Id", new { Id = id });
            }
        }
        public async Task<int> CreateCourse(Course course)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.ExecuteAsync("INSERT INTO Course (Name, Description, CreatedDate, Active) VALUES (@Name, @Description, @CreatedDate, @Active", 
                    new { course.Name, course.Description, course.CreatedDate, course.Active });
            }
        }
    }
}
