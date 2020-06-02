using IdentityService.Domain;
using StudyHard.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyHard.Models
{
    public class HomeModel
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<Role> Roles { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        //TODO: add Interests, upload photo

    }
}
