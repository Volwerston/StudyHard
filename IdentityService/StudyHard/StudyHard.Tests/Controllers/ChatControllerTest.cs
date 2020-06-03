using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyHard.Controllers;
using StudyHard.Controllers.API;
using StudyHard.Helpers;
using StudyHard.Models;
using StudyHard.Service.Dtos;
using StudyHard.Service.Interfaces;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public class ChatControllerTest
    {
        
        private const int USER_ID = 1;
        private const string USER_EMAIL = "email";

        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IChatService> _chatServiceMock;
        private readonly Mock<IUserInfoProvider> _userInfoProviderMock;

        private readonly ChatController _chatController;

        public ChatControllerTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _chatServiceMock = new Mock<IChatService>();
            _userInfoProviderMock = new Mock<IUserInfoProvider>();

            _userInfoProviderMock.Setup(_ => _.GetUserEmail(It.IsAny<ClaimsPrincipal>()))
                .Returns(USER_EMAIL)
                .Verifiable();

            _userRepositoryMock.Setup(_ => _.GetUserIdByEmail("email"))
                .Returns(USER_ID)
                .Verifiable();

            _chatController = new ChatController(_userRepositoryMock.Object,
                _chatServiceMock.Object, _userInfoProviderMock.Object);
        }

        [Fact]
        public void ReturnsCorrectChatId()
        {
            // Given 
            var userChatResponse = new UserChatResponse
            {
                ChatId = 1,
                CollocutorId = 2,
                CollocutorName = "John",
                LastMessage = "Hello",
                LastMessageTime = DateTime.Now
            };
            
            _chatServiceMock
                .Setup(_ => _.GetUserChats(USER_ID))
                .Returns(new List<UserChatResponse> {userChatResponse})
                .Verifiable();

            // When
            var result = _chatController.ChatView(null);

            // Then
            Mock.Verify(_userInfoProviderMock);
            Mock.Verify(_userRepositoryMock);
            Mock.Verify(_chatServiceMock);
            result.Should().BeOfType<ViewResult>();

            var viewResult = (ViewResult) result;
            
            viewResult.Model.Should().BeEquivalentTo(new ChatViewModel
            {
                UserId = USER_ID,
                UserChats = new List<UserChatResponse> {userChatResponse},
                SelectedChatId = userChatResponse.ChatId
            });
        }
    }
}