using FluentAssertions;
using IdentityService.Domain;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StudyHard.Controllers;
using StudyHard.Domain;
using StudyHard.Helpers;
using StudyHard.Models;
using StudyHard.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<ICourseApplicationRepository> _courseApplicationRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserInfoProvider> _userInfoProviderMock;

        private readonly HomeController _controller;
        #region FakeData
        private static int userId => 100500;
        private static int auxiliaryUserId => 100501;
        private static User user = new User
        {
            BirthDate = DateTime.UtcNow,
            Email = "email@email.com",
            Gender = Gender.Male,
            Id = userId,
            Name = "Aaron",
        };
        private static IReadOnlyCollection<Role> roles = new List<Role>
        {
            new Role
            {
                Id = 1,
                Name="Customer"
            },
            new Role
            {
                Id = 2,
                Name="Tutor"
            },
        };
        private static CourseType courseType = new CourseType
        {
            Id = 1,
            Type = "Test type"
        };
        private static List<CourseApplication> courseApplications = new List<CourseApplication>
        {
            new CourseApplication
            {
                ShortDescription = "Sample text",
                Active = true,
                CourseType = courseType,
                CreatedDate = DateTime.UtcNow,
                Name = "Sample name",
                UserId = userId,
                Id = 1
            }
        };

        private static List<Course> coursesForTutor = new List<Course>
        {
            new Course
            {
                Active = true,
                CourseApplicationId = 1,
                CourseType = courseType,
                CourseTypeId = 1,
                CreatedDate = DateTime.UtcNow,
                CustomerId = auxiliaryUserId,
                TutorId = userId,
                Description = "Sample course description",
                Id = 1,
                Name = "Sample name"
            }
        };
        private static List<Course> coursesForCustomer = new List<Course>
        {
            new Course
            {
                Active = true,
                CourseApplicationId = 1,
                CourseType = courseType,
                CourseTypeId = 1,
                CreatedDate = DateTime.UtcNow,
                CustomerId = userId,
                TutorId = auxiliaryUserId,
                Description = "Sample course description",
                Id = 1,
                Name = "Sample name2"
            }
        };
        #endregion

        public HomeControllerTests()
        {
            _courseApplicationRepositoryMock = new Mock<ICourseApplicationRepository>();
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userInfoProviderMock = new Mock<IUserInfoProvider>();
            _controller = new HomeController(_userRepositoryMock.Object, _userInfoProviderMock.Object, _courseApplicationRepositoryMock.Object, _courseRepositoryMock.Object);
        }
        [Fact]
        public async void HasCourseApplications()
        {
            _courseApplicationRepositoryMock
               .Setup(_ => _.GetCourseApplicationsForUser(userId))
               .ReturnsAsync(courseApplications)
               .Verifiable();

            _userInfoProviderMock.Setup(_ => _.IsAuthenticated(It.IsAny<ClaimsPrincipal>())).Returns(true).Verifiable();
            _userRepositoryMock.Setup(_ => _.GetUserIdByEmail(It.IsAny<string>())).Returns(userId).Verifiable();
            _userRepositoryMock.Setup(_ => _.FindUsers(It.IsAny<List<long>>())).Returns(new List<User> { user });
            _userRepositoryMock.Setup(_ => _.FindRoles(It.IsAny<User>())).ReturnsAsync(roles);

            var response = await _controller.Index();
            Mock.Verify(_courseApplicationRepositoryMock,_userInfoProviderMock,_userRepositoryMock);

            var viewResponse = (ViewResult) response;
            ((HomeModel)viewResponse.Model).CourseApplications.Should().BeEquivalentTo(courseApplications);
        }
        [Fact]
        public async void HasCoursesForCustomer()
        {
            
            _courseRepositoryMock.Setup(_ => _.GetCoursesAsCustomer(userId)).ReturnsAsync(coursesForCustomer).Verifiable();
            _userInfoProviderMock.Setup(_ => _.IsAuthenticated(It.IsAny<ClaimsPrincipal>())).Returns(true).Verifiable();
            _userRepositoryMock.Setup(_ => _.GetUserIdByEmail(It.IsAny<string>())).Returns(userId).Verifiable();
            _userRepositoryMock.Setup(_ => _.FindUsers(It.IsAny<List<long>>())).Returns(new List<User> { user });
            _userRepositoryMock.Setup(_ => _.FindRoles(It.IsAny<User>())).ReturnsAsync(roles);

            var response = await _controller.Index();
            Mock.Verify(_courseApplicationRepositoryMock, _userInfoProviderMock, _userRepositoryMock, _courseRepositoryMock);

            var viewResponse = (ViewResult)response;
            ((HomeModel)viewResponse.Model).CoursesAsCustomer.Should().BeEquivalentTo(coursesForCustomer);
        }

        [Fact]
        public async void HasCoursesForTutor()
        {

            _courseRepositoryMock.Setup(_ => _.GetCoursesAsTutor(userId)).ReturnsAsync(coursesForTutor);
            _userInfoProviderMock.Setup(_ => _.IsAuthenticated(It.IsAny<ClaimsPrincipal>())).Returns(true).Verifiable();
            _userRepositoryMock.Setup(_ => _.GetUserIdByEmail(It.IsAny<string>())).Returns(userId).Verifiable();
            _userRepositoryMock.Setup(_ => _.FindUsers(It.IsAny<List<long>>())).Returns(new List<User> { user });
            _userRepositoryMock.Setup(_ => _.FindRoles(It.IsAny<User>())).ReturnsAsync(roles);

            var response = await _controller.Index();
            Mock.Verify(_courseApplicationRepositoryMock, _userInfoProviderMock, _userRepositoryMock, _courseRepositoryMock);

            var viewResponse = (ViewResult)response;
            ((HomeModel)viewResponse.Model).CoursesAsTutor.Should().BeEquivalentTo(coursesForTutor);
        }

        [Fact]
        public void CanSeePersonalInfo()
        {

        }
        [Fact]
        public void CanUpdatePersonalInfo()
        {

        }
    }
}
