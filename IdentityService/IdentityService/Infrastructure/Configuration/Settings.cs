﻿using Microsoft.Extensions.Configuration;

namespace IdentityService.Infrastructure.Configuration
{
    public interface ISettings
    {
        ClientCredentials GoogleCredentials { get; }
        string GoogleServiceUrl { get; }
        string GooglePostLoginRedirectUri { get; }
        string SymmetricSigninKey { get; }
        string DbConnectionString { get; }
    }

    public class Settings : ISettings
    {
        private readonly IConfiguration _configuration;

        public Settings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ClientCredentials GoogleCredentials => new ClientCredentials
        {
            ClientId = _configuration.GetValue<string>("Google:ClientId"),
            ClientSecret = _configuration.GetValue<string>("Google:ClientSecret")
        };

        public string GoogleServiceUrl => _configuration.GetValue<string>("Google:ServiceUrl");
        public string GooglePostLoginRedirectUri => _configuration.GetValue<string>("Google:PostLoginRedirectUri");

        public string SymmetricSigninKey => _configuration.GetValue<string>("SymmetricSigninKey");
        public string DbConnectionString => _configuration.GetValue<string>("DbConnectionString");
    }
}
