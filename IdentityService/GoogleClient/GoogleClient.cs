using Common.System;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GoogleClient
{
    public class GoogleSettings
    {
        public string ServiceUrl { get; set; }
    }

    public class GoogleClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleSettings _settings;

        public GoogleClient(GoogleSettings settings)
        {
            _settings = settings;
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public class ChallengeRequest
        {
            public string Code { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string RedirectUri { get; set; }
        }

        public class ChallengeResponse
        {
            public string AccessToken { get; set; }
        }

        public async Task<Result<ChallengeResponse>> Challenge(ChallengeRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_settings.ServiceUrl}/token"));
            httpRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = request.Code,
                ["client_id"] = request.ClientId,
                ["client_secret"] = request.ClientSecret,
                ["redirect_uri"] = request.RedirectUri,
                ["grant_type"] = "authorization_code"
            });

            var response = await _httpClient.SendAsync(httpRequest);

            var stringResponseBody = await response.Content.ReadAsStringAsync();
            var responseBody = JObject.Parse(stringResponseBody);

            return new ChallengeResponse
            {
                AccessToken = responseBody["id_token"].Value<string>()
            };
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
