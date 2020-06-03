using System.Threading.Tasks;
using Google.Apis.Auth;

namespace IdentityService.Helpers
{
    public interface IGoogleSignatureValidator
    {
        Task<GoogleJsonWebSignature.Payload> Validate(string accessToken, GoogleJsonWebSignature.ValidationSettings settings);
    }

    public class GoogleSignatureValidator : IGoogleSignatureValidator
    {
        public Task<GoogleJsonWebSignature.Payload> Validate(string accessToken, GoogleJsonWebSignature.ValidationSettings settings)
            => GoogleJsonWebSignature.ValidateAsync(accessToken, settings);
    }
}
