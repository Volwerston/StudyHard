using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyHard.Controllers;
using StudyHard.Domain;
using StudyHard.Helpers;
using StudyHard.Models;
using StudyHard.Persistence.Interfaces;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public partial class CourseApplicationControllerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<ICourseApplicationRepository> _courseApplicationRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserInfoProvider> _userInfoProviderMock;
        private readonly Mock<ITutorRepository> _tutorRepositoryMock;

        private readonly CourseApplicationController _courseApplicationController;

        public CourseApplicationControllerTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _courseApplicationRepositoryMock = new Mock<ICourseApplicationRepository>();
            _tutorRepositoryMock = new Mock<ITutorRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userInfoProviderMock = new Mock<IUserInfoProvider>();
            
            _courseApplicationController = new CourseApplicationController(
                _courseRepositoryMock.Object,
                _courseApplicationRepositoryMock.Object,
                _tutorRepositoryMock.Object,
                _userRepositoryMock.Object,
                _userInfoProviderMock.Object);
        }

        [Fact]
        public async Task ReturnsCorrectCourseTypes()
        {
            // Given 
            var courseTypes = new[]
            {
                new CourseType
                {
                    Id = 1,
                    Type = "Maths"
                }
            };

            _courseRepositoryMock
                .Setup(_ => _.GetCourseTypes())
                .ReturnsAsync(courseTypes)
                .Verifiable();

            // When
            var result = await _courseApplicationController.New();

            // Then
            Mock.Verify(_courseRepositoryMock);
            result.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult) result;
            viewResult.Model.Should().BeOfType<CourseApplicationController.CourseApplicationViewModel>();

            var viewModel = (CourseApplicationController.CourseApplicationViewModel) viewResult.Model;

            viewModel.CourseTypes.Should().BeEquivalentTo(courseTypes);
        }

        [Fact]
        public void ReturnsCorrectSearchedCourses()
        {
            // Given 
            var name = "Name";
            var courseTypeIds = new List<int> {1};
            var pageNumber = 1;
            var pageSize = 1;

            var courseApplication = new CourseApplication
            {
                Id = 1,
                Active = true,
                CourseType = new CourseType
                {
                    Id = 1,
                    Type = "Biology"
                },
                CreatedDate = DateTime.Now,
                Name = "Name",
                ShortDescription = "Short description"
            };

            _courseApplicationRepositoryMock
                .Setup(_ => _.Search(It.IsAny<long>(), name, courseTypeIds))
                .Returns(new List<CourseApplication> {courseApplication})
                .Verifiable();

            // When
            var result = _courseApplicationController.GetCourses(name, courseTypeIds, pageNumber, pageSize);

            // Then
            Mock.Verify(_courseRepositoryMock);
            result.Should().BeOfType<OkObjectResult>();

            var objectResult = (OkObjectResult) result;
            objectResult.Value.Should().BeOfType<List<CourseApplication>>();
            ((List<CourseApplication>) objectResult.Value).Should().Contain(courseApplication);
        }

        [Fact]
        public void ReturnsCorrectRequestedCourseTypes()
        {
            // Given 
            var courseTypes = new[]
            {
                new CourseType
                {
                    Id = 1,
                    Type = "Maths"
                }
            };

            _courseRepositoryMock
                .Setup(_ => _.GetCourseTypes())
                .ReturnsAsync(courseTypes)
                .Verifiable();

            // When
            var result = _courseApplicationController.Search();

            // Then
            Mock.Verify(_courseRepositoryMock);
            result.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult) result;
            viewResult.Model.Should().BeOfType<CASearchViewModel>();

            var viewModel = (CASearchViewModel) viewResult.Model;

            viewModel.Skills.Should().BeEquivalentTo(courseTypes);
        }
    }
}