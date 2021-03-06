﻿using Dapper;
using StudyHard.Domain;
using StudyHard.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
        public async Task<List<Course>> GetCoursesAsTutor(long userId)
        {
                using (var connection = new SqlConnection(_connectionString))
                {
                    return connection.Query<Course, CourseType, Course>(
                       @"SELECT 
                        	C.Id, 
                        	C.Name, 
                        	C.Description, 
                        	C.CustomerId, 
							C.TutorId, 
                        	C.CreatedDate, 
                        	C.Active,
                        	CT.Id,
                        	CT.Type
                        FROM [dbo].Course C
                        INNER JOIN [dbo].[CourseType] CT ON C.CourseTypeId = CT.Id
                        WHERE C.TutorId = @userId
                        ORDER BY C.CreatedDate",
                       (c, ct) =>
                       {
                           c.CourseType = ct;
                           return c;
                       },
                       new { userId },
                       splitOn: "Id"
                   ).ToList();
                }
        }

        public async Task<List<Course>> GetCoursesAsCustomer(long userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Course, CourseType, Course>(
                   @"SELECT 
                        	C.Id, 
                        	C.Name, 
                        	C.Description, 
                        	C.CustomerId, 
							C.TutorId, 
                        	C.CreatedDate, 
                        	C.Active,
                        	CT.Id,
                        	CT.Type
                        FROM [dbo].Course C
                        INNER JOIN [dbo].[CourseType] CT ON C.CourseTypeId = CT.Id
                        WHERE C.CustomerId = @userId
                        ORDER BY C.CreatedDate",
                   (c, ct) =>
                   {
                       c.CourseType = ct;
                       return c;
                   },
                   new { userId },
                   splitOn: "Id"
               ).ToList();
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
                return await db.QuerySingleAsync<int>(
                    @"INSERT INTO Course (Name, Description, CreatedDate, Active, CourseTypeId, CourseApplicationId, CustomerId, TutorId) 
                                     VALUES (@Name, @Description, @CreatedDate, @Active, @CourseTypeId, @CourseApplicationId, @CustomerId, @TutorId);
                    SELECT IDENT_CURRENT('dbo.Course');",
                    course);
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

        public async Task<IReadOnlyCollection<CourseType>> GetCourseTypes()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return (await db.QueryAsync<CourseType>("SELECT * FROM CourseType")).ToArray();
            }
        }

        public async Task AddCourseBlog(CourseBlog blog)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                await db.ExecuteAsync(@"INSERT INTO [dbo].[CourseBlogs] (CourseId, AuthorId, CreationDateTimeUtc, Text)
                                     VALUES (@CourseId, @AuthorId, @CreationDateTimeUtc, @Text)",
                    blog);
            }
        }

        public async Task<IReadOnlyCollection<CourseBlog>> GetCourseBlogs(int courseId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return (await db.QueryAsync<CourseBlog>(
                        @"SELECT CB.Id, CB.CourseId, CB.AuthorId, U.Name as AuthorName, CB.CreationDateTimeUtc, CB.Text
                             FROM [dbo].[CourseBlogs] CB
                             INNER JOIN [dbo].[User] U
                             ON CB.AuthorId = U.Id
                             WHERE CB.CourseId = @courseId",
                        new { courseId }))
                    .ToArray();
            }
        }
    }
}