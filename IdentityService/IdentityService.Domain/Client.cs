namespace IdentityService.Domain
{
    public class Client
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }
}
