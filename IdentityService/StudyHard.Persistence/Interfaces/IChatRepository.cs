using System.Collections.Generic;
using StudyHard.Persistence.Entities;

namespace StudyHard.Persistence.Interfaces
{
    public interface IChatRepository
    {
        // Chats
        ChatEntity GetChatById(long id);
        ChatEntity GetChatByUsers(long userId1, long userId2);
        List<ChatEntity> GetChatsByUser(long userId);
        ChatEntity CreateChat(long userId1, long userId2);
        
        // Messages
        List<MessageEntity> GetMessages(long chatId);
        List<MessageEntity> GetLatestMessages(List<long> chatId);
        MessageEntity GetMessageById(long id);
        MessageEntity SaveMessage(long chatId, long sentById, string content);
    }
}