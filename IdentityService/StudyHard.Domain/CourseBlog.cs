using System;

namespace StudyHard.Domain
{
    public class CourseBlog
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreationDateTimeUtc { get; set; }
        public string Text { get; set; }
    }
}