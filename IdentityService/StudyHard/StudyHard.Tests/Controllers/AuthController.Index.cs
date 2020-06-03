using System.Threading.Tasks;
using Common.System;
using FluentAssertions;
using GoogleClient;
using IdentityService.Controllers;
using IdentityService.Helpers;
using IdentityService.Infrastructure.Configuration;
using IdentityService.Models;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public partial class AuthControllerTests
    {
        private readonly Mock<ISettings> _settingsMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<GoogleClient.IGoogleClient> _googleClientMock;
        private readonly Mock<IGoogleSignatureValidator> _validatorMock;

        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _settingsMock = new Mock<ISettings>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _googleClientMock = new Mock<IGoogleClient>();
            _validatorMock = new Mock<IGoogleSignatureValidator>();

            _authController = new AuthController(
                _settingsMock.Object,
                _userRepositoryMock.Object,
                _googleClientMock.Object,
                _validatorMock.Object);
        }

        [Fact]
        public void ShouldReturnCorrectViewModel()
        {
            // Given
            const string postLoginRedirectUri = "PostLoginRedirectUri";
            const string redirectUri = "SomeRedirectUri";

            var googleCredential = new ClientCredentials
            {
                ClientId = "client_id"
            };

            _settingsMock
                .Setup(_ => _.GoogleCredentials)
                .Returns(googleCredential)
                .Verifiable();

            _settingsMock
                .Setup(_ => _.GooglePostLoginRedirectUri)
                .Returns(postLoginRedirectUri)
                .Verifiable();

            // When
            var response = _authController.Index(new AuthController.AuthRequestModel
            {
                RedirectUri = redirectUri
            });

            // Then
            response.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult) response;
            viewResult.Model.Should().BeOfType<AuthViewModel>();

            var viewModel = (AuthViewModel) viewResult.Model;

            viewModel.State.Should().Be(redirectUri.ToBase64());
            viewModel.GoogleCredentials.ClientId.Should().Be(googleCredential.ClientId);
            viewModel.GoogleCredentials.PostLoginRedirectUrl.Should().Be(postLoginRedirectUri);
        }
    }
}
