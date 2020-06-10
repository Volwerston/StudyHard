using System;
using System.Collections.Generic;
using System.Linq;
using IdentityService.Persistence.Interfaces;
using StudyHard.Persistence.Entities;
using StudyHard.Persistence.Interfaces;
using StudyHard.Service.Dtos;
using StudyHard.Service.Interfaces;

namespace StudyHard.Service.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repository;
        private readonly IUserRepository _userRepository;

        public ChatService(IChatRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public List<UserChatResponse> GetUserChats(long userId)
        {
            var chats = _repository.GetChatsByUser(userId);

            var chatIds = chats.Select(c => c.Id).ToList();
            var latestMessagesDict = _repository.GetLatestMessages(chatIds)
                .ToDictionary(me => me.ChatId, me => me);

            var collocutorIds = chats.Select(c => GetCollocutorId(c, userId)).ToList();
            var userNameDict = _userRepository.FindUsers(collocutorIds)
                .ToDictionary(user => user.Id, user => user.Name);
            var userPictureDict = _userRepository.FindUsers(collocutorIds).ToDictionary(user => user.Id, user => user.PictureUrl);
            return chats.Select(chat =>
                    new UserChatResponse
                    {
                        ChatId = chat.Id,
                        CollocutorId = GetCollocutorId(chat, userId),
                        CollocutorName = userNameDict[GetCollocutorId(chat, userId)],
                        CollocutorPictureUrl = userPictureDict[GetCollocutorId(chat, userId)],
                        LastMessage = latestMessagesDict.ContainsKey(chat.Id) ?
                            latestMessagesDict[chat.Id].Content :
                            "No messages yet",
                        LastMessageTime = latestMessagesDict.ContainsKey(chat.Id) ?
                            latestMessagesDict[chat.Id].SentDateTime :
                            chat.Created
                    })
                .OrderByDescending(ucr => ucr.LastMessageTime)
                .ToList();
        }

        public long InitiateChatWithUser(long userId, long collocutorId)
        {
            ChatEntity chatEntity = _repository.GetChatByUsers(userId, collocutorId);

            if (chatEntity == null)
            {
                chatEntity = _repository.CreateChat(userId, collocutorId);
            }

            return chatEntity.Id;
        }

        public MessageResponse SaveMessage(long senderId, SendMessageRequest sendMessageRequest)
        {

            ChatEntity chatEntity = _repository.GetChatById(sendMessageRequest.ChatId);

            if (chatEntity == null)
            {
                throw new Exception("Chat not found");
            }

            var message = _repository.SaveMessage(chatEntity.Id, senderId, sendMessageRequest.Content);

            return new MessageResponse
            {
                Id = message.Id,
                Content = message.Content,
                SendTime = message.SentDateTime,
                SenderId = message.SentBy,
                ReceiverId = GetCollocutorId(chatEntity, message.SentBy)
            };
        }

        public List<MessageResponse> GetChatMessages(long userId, long chatId)
        {
            ChatEntity chatEntity = _repository.GetChatById(chatId);
            if (chatEntity == null || (chatEntity.UserId1 != userId && chatEntity.UserId2 != userId))
            {
                throw new Exception("Chat not found");
            }

            return _repository.GetMessages(chatId)
                .Select(me => new MessageResponse
                {
                    Id = me.Id,
                    Content = me.Content,
                    SendTime = me.SentDateTime,
                    SenderId = me.SentBy,
                    ReceiverId = GetCollocutorId(chatEntity, me.SentBy)
                })
                .OrderBy(mr => mr.SendTime)
                .ToList();
        }

        private long GetCollocutorId(ChatEntity chatEntity, long userId)
        {
            return userId == chatEntity.UserId1 ? chatEntity.UserId2 : chatEntity.UserId1;
        }
    }
}