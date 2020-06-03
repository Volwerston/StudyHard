using System;
using System.Collections.Generic;
using FluentAssertions;
using IdentityService.Domain;
using IdentityService.Persistence.Interfaces;
using Xunit;
using StudyHard.Service;
using Moq;
using StudyHard.Persistence.Entities;
using StudyHard.Persistence.Interfaces;
using StudyHard.Service.Dtos;
using StudyHard.Service.Implementations;
using StudyHard.Service.Interfaces;

namespace StudyHard.Tests.Services
{
    public class ChatServiceTests
    {
        private readonly Mock<IChatRepository> _chatRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        private readonly ChatService _chatService;

        public ChatServiceTests()
        {
            _chatRepositoryMock = new Mock<IChatRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _chatService = new ChatService(_chatRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void ShouldReturnCorrectUserChatResponse()
        {
            //Given
            const int userId = 123;
            const int userId2 = 124;
            var userChat = new ChatEntity {Id = 1, UserId1 = userId, UserId2 = userId2};
            var chatEntities = new List<ChatEntity>() {userChat};
            var latestMessageOne = new MessageEntity {ChatId = userChat.Id};
            var userOne = new User {Id = userId, Name = "Test1"};
            var userTwo = new User {Id = userId2, Name = "Test2"};


            _chatRepositoryMock.Setup(_ => _.GetChatsByUser(userId)).Returns(chatEntities);
            _chatRepositoryMock.Setup(_ => _.GetLatestMessages(new List<long>() {userChat.Id}))
                .Returns(new List<MessageEntity>() {latestMessageOne});
            _userRepositoryMock.Setup(_ => _.FindUsers(new List<long>() {userId2}))
                .Returns(new List<User>() {userTwo});

            //When
            var userChatResponses = _chatService.GetUserChats(userId);

            //Then
            userChatResponses.Count.Should().Be(chatEntities.Count);
            userChatResponses[0].ChatId.Should().Be(userChat.Id);
        }

        [Fact]
        public void ShouldInitiateExistedChatWithUser()
        {
            //Given
            const int userId = 123;
            const int colocutorId = 124;
            var userChat = new ChatEntity {Id = 1, UserId1 = userId, UserId2 = colocutorId};

            _chatRepositoryMock.Setup(_ => _.GetChatByUsers(userId, colocutorId)).Returns(userChat);

            //When
            var userChatId = _chatService.InitiateChatWithUser(userId, colocutorId);

            //Then
            userChatId.Should().Be(userChat.Id);
        }

        [Fact]
        public void ShouldInitiateNewChatWithUser()
        {
            //Given
            const int userId = 123;
            const int colocutorId = 124;
            var userChat = new ChatEntity {Id = 1, UserId1 = userId, UserId2 = colocutorId};

            _chatRepositoryMock.Setup(_ => _.GetChatByUsers(userId, colocutorId)).Returns((ChatEntity) null);
            _chatRepositoryMock.Setup(_ => _.CreateChat(userId, colocutorId)).Returns(userChat);

            //When
            var userChatId = _chatService.InitiateChatWithUser(userId, colocutorId);

            //Then
            userChatId.Should().Be(userChat.Id);
        }

        [Fact]
        public void ShouldSaveMessage()
        {
            //Given
            const int senderId = 123;
            const int colocutorId = 124;
            const int chatId = 1;
            var sendMessageRequest = new SendMessageRequest {ChatId = chatId, Content = "Content"};
            var userChat = new ChatEntity {Id = chatId, UserId1 = senderId, UserId2 = colocutorId};
            var messageEntity = new MessageEntity
            {
                Id = 12, Content = sendMessageRequest.Content, SentDateTime = DateTime.Now, SentBy = senderId
            };

            _chatRepositoryMock.Setup(_ => _.GetChatById(chatId)).Returns(userChat);
            _chatRepositoryMock.Setup(_ => _.SaveMessage(userChat.Id, senderId, sendMessageRequest.Content))
                .Returns(messageEntity);

            //When
            var messageResponse = _chatService.SaveMessage(senderId, sendMessageRequest);

            //Then
            messageResponse.Id.Should().Be(messageEntity.Id);
            messageResponse.Content.Should().Be(messageEntity.Content);
            messageResponse.ReceiverId.Should().Be(colocutorId);
        }
        
        [Fact]
        public void ShouldThrowExceptionOnSaveMessage()
        {
            //Given
            const int senderId = 123;
            const int chatId = 1;
            var sendMessageRequest = new SendMessageRequest {ChatId = chatId, Content = "Content"};

            _chatRepositoryMock.Setup(_ => _.GetChatById(chatId)).Returns((ChatEntity) null);

            //When
            Action act = () => _chatService.SaveMessage(senderId, sendMessageRequest);

            //Then
            var exception = Assert.Throws<Exception>(act);
            Assert.Equal("Chat not found", exception.Message);
        }
        
        [Fact]
        public void ShouldThrowExceptionOnGetChatMessages()
        {
            //Given
            const int userId = 123;
            const int chatId = 1;

            _chatRepositoryMock.Setup(_ => _.GetChatById(chatId)).Returns((ChatEntity) null);

            //When
            Action act = () => _chatService.GetChatMessages(userId, chatId);

            //Then
            var exception = Assert.Throws<Exception>(act);
            Assert.Equal("Chat not found", exception.Message);
        }
        
        [Fact]
        public void ShouldGetChatMessages()
        {
            //Given
            const int userId = 123;
            const int colocutorId = 124;
            const int chatId = 1;
            var userChat = new ChatEntity {Id = chatId, UserId1 = userId, UserId2 = colocutorId};
            var messageEntity = new MessageEntity()
                {Id = 1, ChatId = chatId, Content = "Content", SentBy = userId, SentDateTime = DateTime.Now};
            var messageEntities = new List<MessageEntity>(){messageEntity};

            _chatRepositoryMock.Setup(_ => _.GetChatById(chatId)).Returns(userChat);
            _chatRepositoryMock.Setup(_ => _.GetMessages(chatId)).Returns(messageEntities);

            //When
            var messageResponses = _chatService.GetChatMessages(userId, chatId);

            //Then
            messageResponses.Count.Should().Be(messageEntities.Count);
            messageResponses[0].Id.Should().Be(messageEntity.Id);
            messageResponses[0].Content.Should().Be(messageEntity.Content);
            messageResponses[0].SenderId.Should().Be(messageEntity.SentBy);
            messageResponses[0].ReceiverId.Should().Be(colocutorId);
        }
    }
}