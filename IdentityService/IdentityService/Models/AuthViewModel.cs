namespace IdentityService.Models
{
    public class ClientCredentialsViewModel
    {
        public string ClientId { get; set; }
        public string PostLoginRedirectUrl { get; set; }
    }

    public class AuthViewModel
    {
        public string State { get; set; }
        public ClientCredentialsViewModel GoogleCredentials { get; set; }
    }
}
