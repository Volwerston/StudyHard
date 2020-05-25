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
        public async Task ShouldReturnNotFoundWhenCourseApplicationDoesNotExist()
        {
            // When
            var result = await _courseApplicationController.Index(123);

            // Then
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ShouldReturnCorrectCourseApplication()
        {
            // Given
            var courseApplicationId = 123;
            var courseApplication = new CourseApplication
            {
                Id = courseApplicationId,
                Name = "Some name",
                ShortDescription = "Some description",
                Active = true,
                CreatedDate = DateTime.UtcNow,
                CourseType = new CourseType
                {
                    Id = 1,
                    Type = "Maths"
                },
                UserId = 456
            };

            _courseApplicationRepositoryMock
                .Setup(_ => _.Find(courseApplicationId))
                .ReturnsAsync(courseApplication)
                .Verifiable();

            // When
            var result = await _courseApplicationController.Index(courseApplicationId);

            // Then
            Mock.Verify(_courseApplicationRepositoryMock);
            
            result.Should().BeOfType<ViewResult>();
            var viewResult = (ViewResult) result;

            viewResult.Model.Should().BeOfType<CourseApplicationController.CourseApplicationInfoViewModel>();
            var viewModel = (CourseApplicationController.CourseApplicationInfoViewModel) viewResult.Model;

            viewModel.Application.Id.Should().Be(courseApplication.Id);
            viewModel.Application.Name.Should().Be(courseApplication.Name);
            viewModel.Application.ShortDescription.Should().Be(courseApplication.ShortDescription);
            viewModel.Application.UserId.Should().Be(courseApplication.UserId);
            viewModel.Application.Active.Should().Be(viewModel.Application.Active);
            viewModel.Application.CreatedDate.Should().Be(courseApplication.CreatedDate);
            viewModel.Application.CourseType.Id.Should().Be(courseApplication.CourseType.Id);
            viewModel.Application.CourseType.Type.Should().Be(courseApplication.CourseType.Type);
        }
    }
}
