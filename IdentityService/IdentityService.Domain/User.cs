using System;

namespace IdentityService.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; } //TO-DO: consider enum?
        public DateTime? BirthDate { get; set; }
    }
}
