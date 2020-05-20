using System;

namespace StudyHard.Domain
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreationDateTimeUtc { get; set; }
    }
}
