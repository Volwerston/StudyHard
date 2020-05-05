using IdentityService.Domain;
using StudyHard.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyHard.Models
{
    //Set as an example, usually models will be quite different from the entities
    public class CourseListModel
    {
        public List<Course> Courses { get; set; }
    }
}
