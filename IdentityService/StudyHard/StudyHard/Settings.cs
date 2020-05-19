using Microsoft.Extensions.Configuration;

namespace StudyHard
{
    public class Settings
    {
        private IConfiguration configuration;
        public Settings(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public string IdentityServiceURL => configuration.GetValue<string>("IdentityServiceURL");

        public string SymmetricSigninKey => configuration.GetValue<string>("SymmetricSigninKey");
    }
}
