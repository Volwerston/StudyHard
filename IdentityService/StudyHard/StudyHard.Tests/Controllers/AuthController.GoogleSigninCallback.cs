using System.Threading.Tasks;
using Common.System;
using FluentAssertions;
using Google.Apis.Auth;
using IdentityService.Controllers;
using IdentityService.Domain;
using IdentityService.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public partial class AuthControllerTests
    {
        [Fact]
        public async Task ShouldStoreCorrectUser()
        {
            // Given
            var googleCredential = new ClientCredentials
            {
                ClientId = "client_id",
                ClientSecret = "client_secret"
            };

            _settingsMock
                .Setup(_ => _.GoogleCredentials)
                .Returns(googleCredential)
                .Verifiable();

            _settingsMock
                .Setup(_ => _.SymmetricSigninKey)
                .Returns("SomeKey1234567890")
                .Verifiable();

            _googleClientMock
                .Setup(_ => _.Challenge(It.IsAny<GoogleClient.GoogleClient.ChallengeRequest>()))
                .ReturnsAsync(Result<GoogleClient.GoogleClient.ChallengeResponse>.Ok(
                    new GoogleClient.GoogleClient.ChallengeResponse
                    {
                        AccessToken = "access_token"
                    }))
                .Verifiable();

            const string userName = "Name";
            const string userEmail = "Email";

            _validatorMock
                .Setup(_ => _.Validate(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>()))
                .ReturnsAsync(new GoogleJsonWebSignature.Payload
                {
                    Name = userName,
                    Email = userEmail
                })
                .Verifiable();

            User actualUser = null;

            _userRepositoryMock
                .Setup(_ => _.Save(It.IsAny<User>()))
                .Returns(Task.CompletedTask)
                .Callback((User u) => actualUser = u)
                .Verifiable();

            _userRepositoryMock
                .Setup(_ => _.FindRoles(It.IsAny<User>()))
                .ReturnsAsync(new[]
                {
                    new Role
                    {
                        Id = 1,
                        Name = "Role1"
                    }
                })
                .Verifiable();

            // When
            var response = await _authController.GoogleSigninCallback(
                new AuthController.GoogleSigninCallbackRequestModel
                {
                    State = "abcdef".ToBase64()
                });

            // Then
            Mock.Verify(
                _googleClientMock,
                _validatorMock,
                _userRepositoryMock,
                _settingsMock);

            response.Should().BeOfType<RedirectResult>();
            actualUser.Name.Should().Be(userName);
            actualUser.Email.Should().Be(userEmail);
        }
    }
}
