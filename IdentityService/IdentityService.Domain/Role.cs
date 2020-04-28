namespace IdentityService.Domain
{
    public static class Roles
    {
        public static string Regular = "Regular";
        public static string Tutor = "Tutor";
    }

    public class Role
    {
        public string Name { get; set; }
    }
}
