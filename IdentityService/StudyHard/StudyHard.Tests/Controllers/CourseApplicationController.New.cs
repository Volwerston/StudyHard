using System.Threading.Tasks;
using FluentAssertions;
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
    public partial class CourseApplicationControllerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<ICourseApplicationRepository> _courseApplicationRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserInfoProvider> _userInfoProviderMock;

        private readonly CourseApplicationController _courseApplicationController;

        public CourseApplicationControllerTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _courseApplicationRepositoryMock = new Mock<ICourseApplicationRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userInfoProviderMock = new Mock<IUserInfoProvider>();

            _courseApplicationController = new CourseApplicationController(
                _courseRepositoryMock.Object,
                _courseApplicationRepositoryMock.Object,
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
    }
}
