using System;

namespace StudyHard.Persistence.Entities
{
    public class MessageEntity
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public DateTime SentDateTime { get; set; }
        
        public long ChatId { get; set; }
        public long SentBy { get; set; }
    }
}