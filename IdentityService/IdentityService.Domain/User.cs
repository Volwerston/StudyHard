using StudyHard.Domain;
using System;

namespace IdentityService.Domain
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PictureUrl { get; set; }
    }
}
