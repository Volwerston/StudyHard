using System;

namespace StudyHard.Service.Dtos
{
    public class UserChatResponse
    {
        public long ChatId { get; set; }
        
        public long CollocutorId { get; set; }
        public string CollocutorName { get; set; }
        
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
    }
}