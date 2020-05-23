using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyHard.Domain;
using StudyHard.Models;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public partial class TutorControllerTests
    {
        [Fact]
        public async Task ShouldReturnViewModelWithCourseTypes()
        {
            // Given
            var courseTypes = new[]
            {
                new CourseType
                {
                    Id = 1,
                    Type = "Math"
                }
            };

            _courseRepositoryMock
                .Setup(_ => _.GetCourseTypes())
                .ReturnsAsync(courseTypes)
                .Verifiable();

            // When
            var result = await _tutorController.Search();

            // Then
            Mock.Verify(_courseRepositoryMock);
            result.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult) result;
            viewResult.Model.Should().BeOfType<TutorSearchViewModel>();

            var viewModel = (TutorSearchViewModel) viewResult.Model;
            viewModel.Skills.Should().BeEquivalentTo(courseTypes.Select(x => x.Type));
        }
    }
}
