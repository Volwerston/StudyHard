using System;

namespace StudyHard.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public int CustomerId { get; set; }
        public int TutorId { get; set; }
        public int CourseApplicationId { get; set; }
        public int CourseTypeId { get; set; }
    }
}