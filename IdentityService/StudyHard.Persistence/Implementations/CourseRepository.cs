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
                return await db.QueryFirstOrDefaultAsync<Course>("SELECT * FROM Course WHERE Id = @Id", new {Id = id});
            }
        }

        public async Task<int> CreateCourse(Course course)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.ExecuteAsync(
                    "INSERT INTO Course (Name, Description, CreatedDate, Active) VALUES (@Name, @Description, @CreatedDate, @Active",
                    new {course.Name, course.Description, course.CreatedDate, course.Active});
            }
        }

        public async Task<int> CreateCourseApplication(string name, string shortDescription, int courseTypeId,
            int userId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.ExecuteAsync("INSERT INTO CourseApplication " +
                                             " (Name, ShortDescription, ApplicantId, CourseTypeId, CreatedDate, Active)" +
                                             " VALUES (@Name, @ShortDescription, @ApplicantId, @CourseTypeId, CreatedDate, Active)",
                    new {Name = name, ShortDescription = shortDescription, ApplicantId = userId, CourseTypeId = courseTypeId,
                        CreatedDate = DateTime.Now, Active = true});
            }
        }

        // public async Task<List<CourseApplication>> GetApplications()
        // {
        //     using (IDbConnection db = new SqlConnection(_connectionString))
        //     {
        //         db.QueryAsync<CourseApplication>(
        //             "SELECT ca.Id, ca.Name, ca.ShortDescription, ca.ApplicantId, ca.CourseTypeId, ct.Type, ca.CreatedDate" +
        //             " FROM CourseApplication ca INNER JOIN CourseType ct on ca.CourseTypeId = ct.Id WHERE ca.Active = true",
        //             (caId, caName, caShortDescription, caApplicantId, caCourseTypeId, ctType, caCreatedDate) =>
        //             {
        //                 return new CourseApplication
        //                 {
        //                     
        //                 };
        //             });
        //     }
        // }

        public async Task<CourseApplication> GetCourseApplicationById(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<CourseApplication>("SELECT * FROM CourseApplication WHERE Id = @Id",
                    new {Id = id});
            }
        }
    }
}