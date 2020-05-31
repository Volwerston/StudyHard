using System;
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
        public async Task ShouldReturnBadRequestWhenCourseApplicationDoesNotExist()
        {
            // When
            var response = await _courseApplicationController.AcceptApplication(new CourseApplicationController.AcceptCourseApplicationRequest());

            // Then
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ShouldReturnBadRequestWhenTutorDoesNotExist()
        {
            // Given
            _courseApplicationRepositoryMock
                .Setup(_ => _.Find(It.IsAny<int>()))
                .ReturnsAsync(new CourseApplication())
                .Verifiable();

            // When
            var response = await _courseApplicationController.AcceptApplication(new CourseApplicationController.AcceptCourseApplicationRequest());

            // Then
            Mock.Verify(_courseApplicationRepositoryMock);
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ShouldReturnBadRequestWhenCurrentUserIsNotTutor()
        {
            // Given
            _courseApplicationRepositoryMock
                .Setup(_ => _.Find(It.IsAny<int>()))
                .ReturnsAsync(new CourseApplication())
                .Verifiable();

            _tutorRepositoryMock
                .Setup(_ => _.Find(It.IsAny<int>()))
                .ReturnsAsync(new Tutor
                {
                    Id = 123
                })
                .Verifiable();

            _userRepositoryMock
                .Setup(_ => _.GetUserIdByEmail(It.IsAny<string>()))
                .Returns(456)
                .Verifiable();

            // When
            var response = await _courseApplicationController.AcceptApplication(new CourseApplicationController.AcceptCourseApplicationRequest());

            // Then
            Mock.Verify(_courseApplicationRepositoryMock, _tutorRepositoryMock, _userRepositoryMock);
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ShouldReturnBadRequestWhenTutorIdEqualsApplicantId()
        {
            // Given
            _courseApplicationRepositoryMock
                .Setup(_ => _.Find(It.IsAny<int>()))
                .ReturnsAsync(new CourseApplication
                {
                    UserId = 123
                })
                .Verifiable();

            _tutorRepositoryMock
                .Setup(_ => _.Find(It.IsAny<int>()))
                .ReturnsAsync(new Tutor
                {
                    Id = 123
                })
                .Verifiable();

            _userRepositoryMock
                .Setup(_ => _.GetUserIdByEmail(It.IsAny<string>()))
                .Returns(123)
                .Verifiable();

            // When
            var response = await _courseApplicationController.AcceptApplication(new CourseApplicationController.AcceptCourseApplicationRequest
            {
                TutorId = 123
            });

            // Then
            Mock.Verify(_courseApplicationRepositoryMock, _tutorRepositoryMock, _userRepositoryMock);
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ShouldReturnBadRequestWhenTutorDoesNotHaveRequiredSkill()
        {
            // Given
            _courseApplicationRepositoryMock
                .Setup(_ => _.Find(It.IsAny<int>()))
                .ReturnsAsync(new CourseApplication
                {
                    UserId = 456
                })
                .Verifiable();

            _tutorRepositoryMock
                .Setup(_ => _.Find(It.IsAny<int>()))
                .ReturnsAsync(new Tutor
                {
                    Id = 123
                })
                .Verifiable();

            _userRepositoryMock
                .Setup(_ => _.GetUserIdByEmail(It.IsAny<string>()))
                .Returns(123)
                .Verifiable();

            _tutorRepositoryMock
                .Setup(_ => _.GetCourses(It.IsAny<int>()))
                .ReturnsAsync(Array.Empty<CourseType>())
                .Verifiable();

            // When
            var response = await _courseApplicationController.AcceptApplication(new CourseApplicationController.AcceptCourseApplicationRequest
            {
                TutorId = 123
            });

            // Then
            Mock.Verify(_courseApplicationRepositoryMock, _tutorRepositoryMock, _userRepositoryMock);
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ShouldReturnRedirectWhenApplicationSuccessfullyAccepted()
        {
            // Given
            var courseApplication = new CourseApplication
            {
                Id = 101112,
                UserId = 456,
                ShortDescription = "Short description",
                Name = "Some name",
                Active = true,
                CourseType = new CourseType
                {
                    Id = 1,
                    Type = "Maths"
                }
            };

            _courseApplicationRepositoryMock
                .Setup(_ => _.Find(It.IsAny<int>()))
                .ReturnsAsync(courseApplication)
                .Verifiable();

            _tutorRepositoryMock
                .Setup(_ => _.Find(It.IsAny<int>()))
                .ReturnsAsync(new Tutor
                {
                    Id = 123
                })
                .Verifiable();

            _userRepositoryMock
                .Setup(_ => _.GetUserIdByEmail(It.IsAny<string>()))
                .Returns(123)
                .Verifiable();

            _tutorRepositoryMock
                .Setup(_ => _.GetCourses(It.IsAny<int>()))
                .ReturnsAsync(new[]
                {
                    new CourseType
                    {
                        Id = 1,
                        Type = "Maths"
                    }
                })
                .Verifiable();

            Course actualCourse = null;
            var courseId = 123;

            _courseRepositoryMock
                .Setup(_ => _.CreateCourse(It.IsAny<Course>()))
                .ReturnsAsync(courseId)
                .Callback((Course c) => actualCourse = c)
                .Verifiable();

            // When
            var response = await _courseApplicationController.AcceptApplication(new CourseApplicationController.AcceptCourseApplicationRequest
            {
                TutorId = 123,
                CourseApplicationId = courseApplication.Id
            });

            // Then
            Mock.Verify(
                _courseApplicationRepositoryMock, 
                _tutorRepositoryMock, 
                _userRepositoryMock,
                _courseRepositoryMock);

            response.Should().BeOfType<RedirectToActionResult>();

            actualCourse.Name.Should().Be(courseApplication.Name);
            actualCourse.Description.Should().Be(courseApplication.ShortDescription);
            actualCourse.Active.Should().BeTrue();
            actualCourse.CourseTypeId.Should().Be(courseApplication.CourseType.Id);
            actualCourse.CustomerId.Should().Be(courseApplication.UserId);
            actualCourse.TutorId.Should().Be(123);
            actualCourse.CourseApplicationId.Should().Be(courseApplication.Id);
        }
    }
}
