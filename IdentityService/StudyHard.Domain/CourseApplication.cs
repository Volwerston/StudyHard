using System;

namespace StudyHard.Domain
{
    public class CourseApplication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        
        public int UserId { get; set; }
        public CourseType CourseType { get; set; }
    }
}