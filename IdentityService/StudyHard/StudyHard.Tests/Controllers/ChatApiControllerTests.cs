using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyHard.Controllers.API;
using StudyHard.Helpers;
using StudyHard.Service.Dtos;
using StudyHard.Service.Interfaces;
using Xunit;

namespace StudyHard.Tests.Controllers
{
    public class ChatApiControllerTests
    {
        private const int USER_ID = 1;
        private const string USER_EMAIL = "email";

        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IChatService> _chatServiceMock;
        private readonly Mock<IUserInfoProvider> _userInfoProviderMock;

        private readonly ChatApiController _chatApiController;

        public ChatApiControllerTests()
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

            _chatApiController = new ChatApiController(_userRepositoryMock.Object,
                _chatServiceMock.Object, _userInfoProviderMock.Object);
        }

        [Fact]
        public void ReturnsCorrectChatId()
        {
            // Given 
            int collocutorId = 2;


            int chatId = 1;
            _chatServiceMock
                .Setup(_ => _.InitiateChatWithUser(USER_ID, collocutorId))
                .Returns(chatId)
                .Verifiable();

            // When
            var result = _chatApiController.GetChatId(collocutorId);

            // Then
            Mock.Verify(_chatServiceMock);
            Mock.Verify(_userRepositoryMock);
            Mock.Verify(_chatServiceMock);
            result.Should().BeOfType<ObjectResult>();

            var objectResult = (ObjectResult) result;
            objectResult.Value.Should().BeEquivalentTo(new {ChatId = chatId});
        }

        [Fact]
        public void ReturnsCorrectMessages()
        {
            // Given 
            var messageResponse = new MessageResponse
            {
                Id = 1,
                Content = "message",
                SenderId = 1,
                SendTime = DateTime.Now
            };

            int chatId = 1;
            _chatServiceMock
                .Setup(_ => _.GetChatMessages(USER_ID, chatId))
                .Returns(new List<MessageResponse> {messageResponse})
                .Verifiable();

            // When
            var result = _chatApiController.GetChatMessages(chatId);

            // Then
            Mock.Verify(_chatServiceMock);
            Mock.Verify(_userRepositoryMock);
            Mock.Verify(_chatServiceMock);
            result.Should().BeOfType<ObjectResult>();

            var objectResult = (ObjectResult) result;
            objectResult.Value.Should().BeOfType<List<MessageResponse>>();
            ((List<MessageResponse>) objectResult.Value).Should().Contain(messageResponse);
        }

        [Fact]
        public void ReturnNewMessageId()
        {
            // Given 
            var sendMessageRequest = new SendMessageRequest
            {
                ChatId = 1,
                Content = "message"
            };

            var messageResponse = new MessageResponse
            {
                Content = sendMessageRequest.Content,
                ReceiverId = 2,
                SenderId = 1,
                SendTime = DateTime.Now
            };

            _chatServiceMock
                .Setup(_ => _.SaveMessage(USER_ID, sendMessageRequest))
                .Returns(messageResponse)
                .Verifiable();

            // When
            var result = _chatApiController.SendMessage(sendMessageRequest);

            // Then
            Mock.Verify(_chatServiceMock);
            Mock.Verify(_userRepositoryMock);
            Mock.Verify(_chatServiceMock);
            result.Should().BeOfType<ObjectResult>();

            var objectResult = (ObjectResult) result;
            objectResult.Value.Should().BeOfType<MessageResponse>();
            ((MessageResponse) objectResult.Value).Should().BeEquivalentTo(messageResponse);
        }

        [Fact]
        public void ReturnsCorrectChatData()
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
            var result = _chatApiController.GetChatData();

            // Then
            Mock.Verify(_chatServiceMock);
            Mock.Verify(_userRepositoryMock);
            Mock.Verify(_chatServiceMock);
            result.Should().BeOfType<ObjectResult>();

            var objectResult = (ObjectResult) result;
            objectResult.Value.Should().BeOfType<List<UserChatResponse>>();
            ((List<UserChatResponse>) objectResult.Value).Should().Contain(userChatResponse);
        }
    }
}