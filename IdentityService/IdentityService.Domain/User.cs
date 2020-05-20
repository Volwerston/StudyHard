using System;

namespace IdentityService.Domain
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public long Gender { get; set; } //TO-DO: consider enum?
        public DateTime? BirthDate { get; set; }
    }
}
