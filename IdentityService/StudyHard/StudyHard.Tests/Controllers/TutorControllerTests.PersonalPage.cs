using System;
using System.Collections.Generic;
using System.Text;
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
        public async Task ShouldReturnRedirectWhenTutorNotFound()
        {
            // When
            var result = await _tutorController.PersonalPage(123);

            // Then
            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public async Task ShouldReturnTutorProfileViewWhenTutorExists()
        {
            // Given
            var coursesTypes = new []
            {
                new CourseType
                {
                    Id = 1,
                    Type = "Math"
                }
            };

            var blogs = new []
            {
                new Blog
                {
                    CreationDateTimeUtc = DateTime.UtcNow,
                    Id = 1,
                    Title = "Some title",
                    Text = "Some text"
                }
            };

            var tutor = new Tutor
            {
                Email = "test@gmail.com",
                Id = 1,
                Name = "Name Surname",
                Skills = null
            };

            var tutorId = 123;

            _tutorRepositoryMock
                .Setup(_ => _.Find(tutorId))
                .ReturnsAsync(tutor)
                .Verifiable();

            _tutorRepositoryMock
                .Setup(_ => _.GetCourses(tutorId))
                .ReturnsAsync(coursesTypes)
                .Verifiable();

            _tutorRepositoryMock
                .Setup(_ => _.GetBlogs(tutorId))
                .ReturnsAsync(blogs)
                .Verifiable();

            // When
            var result = await _tutorController.PersonalPage(tutorId);

            // Then
            Mock.Verify(_tutorRepositoryMock);
            result.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult) result;
            viewResult.Model.Should().BeOfType<TutorController.TutorPersonalPageViewModel>();

            var viewModel = (TutorController.TutorPersonalPageViewModel)viewResult.Model;

            viewModel.Courses.Should().BeEquivalentTo(coursesTypes);
            viewModel.Blogs.Should().BeEquivalentTo(blogs);
            viewModel.Tutor.Should().NotBeNull();
            viewModel.Tutor.Name.Should().Be(tutor.Name);
            viewModel.Tutor.Id.Should().Be(tutor.Id);
            viewModel.Tutor.Email.Should().Be(tutor.Email);
            viewModel.Tutor.Skills.Should().BeEquivalentTo(tutor.Skills);
        }
    }
}
