using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyHard.Controllers;
using StudyHard.Domain;
using StudyHard.Persistence.Interfaces;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public partial class TutorControllerTests
    {
        private readonly Mock<ITutorRepository> _tutorRepositoryMock;
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly TutorController _tutorController;

        public TutorControllerTests()
        {
            _tutorRepositoryMock = new Mock<ITutorRepository>();
            _courseRepositoryMock = new Mock<ICourseRepository>();

            _tutorController = new TutorController(
                _tutorRepositoryMock.Object,
                _courseRepositoryMock.Object);
        }

        [Fact]
        public async Task ShouldReturnBadRequestIfRequestModelIsNull()
        {
            // When
            var result = await _tutorController.Find(null);

            // Then
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ShouldReturnTutorsForAllCoursesIfRequestedCoursesNullOrEmpty()
        {
            // Given
            _courseRepositoryMock
                .Setup(_ => _.GetCourseTypes())
                .ReturnsAsync(new[]
                {
                    new CourseType
                    {
                        Id = 1,
                        Type = "Maths"
                    }
                })
                .Verifiable();

            var tutors = new[]
            {
                new Tutor
                {
                    Email = "test@gmail.com",
                    Id = 1,
                    Name = "Test user",
                    Skills = new List<CourseType>
                    {
                        new CourseType
                        {
                            Id = 1,
                            Type = "Maths"
                        }
                    }
                }
            };

            _tutorRepositoryMock
                .Setup(_ => _.Find(It.IsAny<string[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(tutors)
                .Verifiable();

            // When
            var result = await _tutorController.Find(new TutorController.FindTutorsRequest());

            // Then
            Mock.Verify(_courseRepositoryMock, _tutorRepositoryMock);

            result.Should().BeOfType<OkObjectResult>();
            var objectResult = (ObjectResult) result;

            objectResult.Value.Should().Be(tutors);
        }
    }
}
