﻿using System;
using System.Collections.Generic;
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
                        new {applicationId},
                        splitOn: "Id"
                    ))
                    .SingleOrDefault();
            }
        }

        public List<CourseApplication> Find(string name, List<int> courseTypes)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string nameClause = string.IsNullOrEmpty(name)
                    ? ""
                    : " AND LOWER(CA.Name) LIKE '%' + @name + '%' ";

                string courseTypesClause = courseTypes.Any()
                    ? " AND CT.Id in @courseTypes "
                    : "";

                var whereClause = " WHERE CA.Active = 1 " + nameClause + courseTypesClause;

                return connection.Query<CourseApplication, CourseType, CourseApplication>(
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
                        INNER JOIN [dbo].[CourseType] CT ON CA.CourseTypeId = CT.Id"
                    + whereClause + " ORDER BY CA.CreatedDate",
                    (ca, ct) =>
                    {
                        ca.CourseType = ct;
                        return ca;
                    },
                    new {name = name, courseTypes = courseTypes},
                    splitOn: "Id"
                ).ToList();
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

        public async Task Deactivate(int courseApplicationId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(
                    @"UPDATE [dbo].[CourseApplication]
                          SET Active = 0
                          WHERE Id = @id",
                    new
                    {
                        id = courseApplicationId
                    });
            }
        }
    }
}