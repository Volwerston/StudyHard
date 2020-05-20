using System;

namespace StudyHard.Service.Dtos
{
    public class MessageResponse
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public DateTime SendTime { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
    }
}