﻿using System.ComponentModel.DataAnnotations;
using Common.System;
using IdentityService.Models;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth;
using System.Security.Claims;
using IdentityService.Infrastructure.Configuration;
using System.Threading.Tasks;
using System.Text;
using System;
using System.Linq;
using IdentityService.Domain;
using IdentityService.Persistence.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using IdentityService.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly ISettings _settings;
        private readonly IUserRepository _userRepository;
        private readonly GoogleClient.IGoogleClient _googleClient;
        private readonly IGoogleSignatureValidator _validator;
        const string DefaultPictureUrl = "https://lh3.googleusercontent.com/-XdUIqdMkCWA/AAAAAAAAAAI/AAAAAAAAAAA/4252rscbv5M/photo.jpg";

        public AuthController(
            ISettings settings, 
            IUserRepository userRepository, 
            GoogleClient.IGoogleClient googleClient,
            IGoogleSignatureValidator validator)
        {
            _settings = settings;
            _userRepository = userRepository;
            _googleClient = googleClient;
            _validator = validator;
        }

        public class AuthRequestModel
        {
            [Required(AllowEmptyStrings = false)]
            public string RedirectUri { get; set; }
        }

        [HttpGet]
        public IActionResult Index([FromQuery] AuthRequestModel model)
        {
            var googleCredentials = _settings.GoogleCredentials;
            
            return View(new AuthViewModel
            {
                State = model.RedirectUri.ToBase64(),
                GoogleCredentials = new ClientCredentialsViewModel
                {
                    ClientId = googleCredentials.ClientId,
                    PostLoginRedirectUrl = _settings.GooglePostLoginRedirectUri
                }
            });
        }

        public class GoogleSigninCallbackRequestModel
        {
            public string State { get; set; }
            public string Code { get; set; }
        }

        [HttpGet]
        [Route("signin-google")]
        public async Task<IActionResult> GoogleSigninCallback([FromQuery] GoogleSigninCallbackRequestModel model)
        {
            var challengeResponse = await _googleClient.Challenge(
                new GoogleClient.GoogleClient.ChallengeRequest
                {
                    ClientId = _settings.GoogleCredentials.ClientId,
                    ClientSecret = _settings.GoogleCredentials.ClientSecret,
                    Code = model.Code,
                    RedirectUri = _settings.GooglePostLoginRedirectUri
                });

            var payload = await _validator.Validate(
                challengeResponse.Value.AccessToken,
                new GoogleJsonWebSignature.ValidationSettings());

            var user = new User
            {
                Email = payload.Email,
                Name = payload.Name,
                PictureUrl = payload.Picture
            };

            await _userRepository.Save(user);
            var roles = await _userRepository.FindRoles(user);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, payload.Email),
                 new Claim(ClaimTypes.Email, payload.Email),
                 new Claim(ClaimTypes.Name, payload.Name), 
                 new Claim(ClaimTypes.Role, string.Join(",", roles.Select(r => r.Name))),
                 new Claim("Picture", payload.Picture ?? DefaultPictureUrl),
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.SymmetricSigninKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                string.Empty,
                string.Empty,
                claims,
                expires: DateTime.Now.AddSeconds(60 * 60),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var redirectUri = model.State.FromBase64();

            return Redirect($"{redirectUri}?token={tokenString}");
        }
    }
}