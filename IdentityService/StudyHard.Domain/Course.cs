using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyHard.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
    }
}
