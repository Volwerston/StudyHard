using System.Collections.Generic;
using StudyHard.Service.Dtos;

namespace StudyHard.Service.Interfaces
{
    public interface IChatService
    {
        List<UserChatResponse> GetUserChats(long userId);
        long InitiateChatWithUser(long userId, long otherUserId);
        MessageResponse SaveMessage(long userId, SendMessageRequest sendMessageRequest);
        List<MessageResponse> GetChatMessages(long userId, long chatId);
    }
}