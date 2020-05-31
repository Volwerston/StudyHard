using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityService.Domain;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyHard.Controllers;
using StudyHard.Domain;
using StudyHard.Helpers;
using StudyHard.Persistence.Interfaces;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public partial class CourseControllerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserInfoProvider> _userInfoProviderMock;
        private readonly Mock<ITutorRepository> _tutorRepositoryMock;

        private readonly CourseController _courseController;

        public CourseControllerTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _tutorRepositoryMock = new Mock<ITutorRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userInfoProviderMock = new Mock<IUserInfoProvider>();

            _courseController = new CourseController(
                _courseRepositoryMock.Object,
                _tutorRepositoryMock.Object,
                _userRepositoryMock.Object,
                _userInfoProviderMock.Object);
        }

        [Fact]
        public async Task ShouldReturnNotFoundWhenCourseDoesNotExist()
        {
            // When
            var response = await _courseController.GetCourseView(123);

            // Then
            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ShouldReturnNotFoundWhenUserDoesNotBelongToCourse()
        {
            // Given
            _courseRepositoryMock
                .Setup(_ => _.GetCourseById(It.IsAny<int>()))
                .ReturnsAsync(new Course
                {
                    TutorId = 123,
                    CustomerId = 456
                })
                .Verifiable();

            _userRepositoryMock
                .Setup(_ => _.GetUserIdByEmail(It.IsAny<string>()))
                .Returns(789)
                .Verifiable();

            // When
            var response = await _courseController.GetCourseView(123);

            // Then
            Mock.Verify(_courseRepositoryMock, _userRepositoryMock);
            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ShouldReturnCorrectCourse()
        {
            // Given

            var courseType = new CourseType
            {
                Id = 1,
                Type = "Math"
            };

            var course = new Course
            {
                Id = 1,
                Active = true,
                CourseApplicationId = 123,
                CourseTypeId = courseType.Id,
                CreatedDate = DateTime.UtcNow,
                CustomerId = 456,
                Description = "Some description",
                Name = "Some name",
                TutorId = 789
            };

            _courseRepositoryMock
                .Setup(_ => _.GetCourseById(course.Id))
                .ReturnsAsync(course)
                .Verifiable();

            _userRepositoryMock
                .Setup(_ => _.GetUserIdByEmail(It.IsAny<string>()))
                .Returns(course.CustomerId)
                .Verifiable();

            _tutorRepositoryMock
                .Setup(_ => _.Find(course.TutorId))
                .ReturnsAsync(new Tutor
                {
                    Id = course.TutorId
                })
                .Verifiable();

            _userRepositoryMock
                .Setup(_ => _.FindUsers(It.IsAny<List<long>>()))
                .Returns(new List<User>
                {
                    new User
                    {
                        Id = course.CustomerId
                    }
                })
                .Verifiable();

            _courseRepositoryMock
                .Setup(_ => _.GetCourseTypes())
                .ReturnsAsync(new[]
                {
                    courseType
                })
                .Verifiable();

            // When
            var response = await _courseController.GetCourseView(course.Id);

            // Then
            Mock.Verify(_courseRepositoryMock, _userRepositoryMock, _tutorRepositoryMock);
            
            response.Should().BeOfType<ViewResult>();
            var viewResponse = (ViewResult) response;

            viewResponse.Model.Should().BeOfType<CourseController.CourseViewModel>();
            var viewModel = (CourseController.CourseViewModel) viewResponse.Model;

            viewModel.Customer.Id.Should().Be(course.CustomerId);
            viewModel.Tutor.Id.Should().Be(course.TutorId);
            viewModel.CourseType.Id.Should().Be(course.CourseTypeId);
            viewModel.Course.Name.Should().Be(course.Name);
            viewModel.Course.Active.Should().Be(course.Active);
            viewModel.Course.CourseApplicationId.Should().Be(course.CourseApplicationId);
            viewModel.Course.Description.Should().Be(course.Description);
            viewModel.Course.Id.Should().Be(course.Id);
            viewModel.Course.CreatedDate.Should().Be(course.CreatedDate);
        }
    }
}
