using System.Collections.Generic;

namespace StudyHard.Domain
{
    public class Tutor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<CourseType> Skills { get; set; }
    }
}
