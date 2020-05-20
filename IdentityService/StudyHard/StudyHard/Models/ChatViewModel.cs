using System.Collections.Generic;
using StudyHard.Service.Dtos;

namespace StudyHard.Models
{
    public class ChatViewModel
    {
        public long SelectedChatId { get; set; }
        public long UserId { get; set; }
        public List<UserChatResponse> UserChats { get; set; }
    }
}