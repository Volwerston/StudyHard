using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyHard.Controllers;
using StudyHard.Domain;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public partial class CourseApplicationControllerTests
    {
        [Fact]
        public async Task ShouldReturnBadRequestWhenRequestModelInvalid()
        {
            // When
            var result = await _courseApplicationController.Create(null);

            // Then
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ShouldReturnBadRequestWhenCourseTypeDoesNotExist()
        {
            // Given
            _courseRepositoryMock
                .Setup(_ => _.GetCourseTypes())
                .ReturnsAsync(Array.Empty<CourseType>())
                .Verifiable();

            // When
            var result = await _courseApplicationController.Create(
                new CourseApplicationController.CreateCourseApplicationRequest
                {
                    CourseTypeId = 123
                });

            // Then
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ShouldStoreCorrectCourseApplication()
        {
            // Given
            var userId = 123;
            var courseTypes = new[]
            {
                new CourseType
                {
                    Id = 1,
                    Type = "Maths"
                }
            };

            var expectedCourseApplication = new CourseApplication
            {
                Id = 456,
                Name = "Some name",
                ShortDescription = "Some description",
                Active = true,
                UserId = userId,
                CourseType = new CourseType
                {
                    Id = 1,
                    Type = "Maths"
                }
            };

            CourseApplication actualCourseApplication = null;

            _courseRepositoryMock
                .Setup(_ => _.GetCourseTypes())
                .ReturnsAsync(courseTypes)
                .Verifiable();

            _userInfoProviderMock
                .Setup(_ => _.GetUserEmail(It.IsAny<ClaimsPrincipal>()))
                .Returns("test@gmail.com")
                .Verifiable();

            _userRepositoryMock
                .Setup(_ => _.GetUserIdByEmail(It.IsAny<string>()))
                .Returns(userId)
                .Verifiable();

            _courseApplicationRepositoryMock
                .Setup(_ => _.Create(It.IsAny<CourseApplication>()))
                .ReturnsAsync(expectedCourseApplication.Id)
                .Callback((CourseApplication a) => actualCourseApplication = a)
                .Verifiable();

            // When
            var result = await _courseApplicationController.Create(
                new CourseApplicationController.CreateCourseApplicationRequest
                {
                    CourseTypeId = courseTypes[0].Id,
                    Name = expectedCourseApplication.Name,
                    ShortDescription = expectedCourseApplication.ShortDescription
                });

            // Then
            Mock.Verify(
                _courseRepositoryMock, 
                _userRepositoryMock, 
                _courseApplicationRepositoryMock,
                _userInfoProviderMock);

            result.Should().BeOfType<RedirectToActionResult>();

            actualCourseApplication.Name.Should().Be(expectedCourseApplication.Name);
            actualCourseApplication.ShortDescription.Should().Be(expectedCourseApplication.ShortDescription);
            actualCourseApplication.UserId.Should().Be(expectedCourseApplication.UserId);
            actualCourseApplication.Active.Should().Be(expectedCourseApplication.Active);
            actualCourseApplication.CourseType.Id.Should().Be(expectedCourseApplication.CourseType.Id);
            actualCourseApplication.CourseType.Type.Should().Be(expectedCourseApplication.CourseType.Type);
        }
    }
}
