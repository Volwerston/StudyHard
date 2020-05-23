using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyHard.Controllers;
using StudyHard.Domain;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public partial class TutorControllerTests
    {
        [Fact]
        public async Task ShouldReturnRedirectResponse()
        {
            // Given
            var tutorId = 123;

            var expectedBlog = new Blog
            {
                Text = "Some text",
                Title = "Some title"
            };

            Blog actualBlog = null;

            _tutorRepositoryMock
                .Setup(_ => _.AddBlog(tutorId, It.IsAny<Blog>()))
                .Returns(Task.CompletedTask)
                .Callback((int _, Blog blog) => actualBlog = blog)
                .Verifiable();

            // When 
            var result = await _tutorController.CreateBlog(new TutorController.CreateBlogRequestModel
            {
                TutorId = tutorId,
                Text = expectedBlog.Text,
                Title = expectedBlog.Title
            });

            // Then
            Mock.Verify(_tutorRepositoryMock);
            result.Should().BeOfType<RedirectToActionResult>();
            actualBlog.Text.Should().Be(expectedBlog.Text);
            actualBlog.Title.Should().Be(expectedBlog.Title);
        }
    }
}
