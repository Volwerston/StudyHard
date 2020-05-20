using System;

namespace StudyHard.Persistence.Entities
{
    public class ChatEntity
    {
        public long Id { get; set; }
        public long UserId1 { get; set; }
        public long UserId2 { get; set; }
        public DateTime Created { get; set; }
    }
}