using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyHard.Controllers;
using StudyHard.Domain;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public partial class CourseControllerTests
    {
        [Fact]
        public async Task ReturnsBadRequestForNotExistingCourse()
        {
            // When
            var response = await _courseController.CreateBlog(new CourseController.CreateBlogRequest());

            // Then
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ReturnsBadRequestForUnauthorizedUser()
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

            // When
            var response = await _courseController.CreateBlog(
                new CourseController.CreateBlogRequest
                {
                    AuthorId = 789
                });

            // Then
            Mock.Verify(_courseRepositoryMock);
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ReturnsBadRequestWhenCurrentUserIsNotAuthor()
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
                .Returns(456)
                .Verifiable();

            // When
            var response = await _courseController.CreateBlog(
                new CourseController.CreateBlogRequest
                {
                    AuthorId = 123
                });

            // Then
            Mock.Verify(_courseRepositoryMock, _userRepositoryMock);
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task SuccessfulCourseBlogCreation()
        {
            // Given
            var courseRequest = new CourseController.CreateBlogRequest
            {
                AuthorId = 123,
                BlogText = "Some text",
                CourseId = 789
            };

            _courseRepositoryMock
                .Setup(_ => _.GetCourseById(It.IsAny<int>()))
                .ReturnsAsync(new Course
                {
                    Id = courseRequest.CourseId,
                    TutorId = courseRequest.AuthorId,
                    CustomerId = 456
                })
                .Verifiable();

            _userRepositoryMock
                .Setup(_ => _.GetUserIdByEmail(It.IsAny<string>()))
                .Returns(courseRequest.AuthorId)
                .Verifiable();

            CourseBlog actualBlog = null;

            _courseRepositoryMock
                .Setup(_ => _.AddCourseBlog(It.IsAny<CourseBlog>()))
                .Returns(Task.CompletedTask)
                .Callback((CourseBlog cb) => actualBlog = cb)
                .Verifiable();

            // When
            var response = await _courseController.CreateBlog(courseRequest);

            // Then
            Mock.Verify(_courseRepositoryMock, _userRepositoryMock);

            response.Should().BeOfType<RedirectToActionResult>();

            actualBlog.Should().NotBeNull();
            actualBlog.AuthorId.Should().Be(courseRequest.AuthorId);
            actualBlog.CourseId.Should().Be(courseRequest.CourseId);
            actualBlog.Text.Should().Be(courseRequest.BlogText);
        }
    }
}
