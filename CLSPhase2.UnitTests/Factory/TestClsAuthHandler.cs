using CLSPhase2.Api.Extensions;
using CLSPhase2.Api.Handlers.AuthHandlers;
using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using CLSPhase2.Api.Handlers.AuthHandlers.Scheme;
using CLSPhase2.Api.Models;
using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.UnitOfWork;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CLSPhase2.UnitTests.Factory
{
    public class TestClsAuthHandler : AuthenticationHandler<ClsAuthSchemeOptions>
    {
        ClsAuthSchemeOptions _options;
        private readonly ApiSettings _apiSetting;
        public TestClsAuthHandler(IOptionsMonitor<ClsAuthSchemeOptions> options,
            IOptions<ApiSettings> apisetting,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _apiSetting = apisetting.Value;
        }
        protected override Task InitializeHandlerAsync()
        {
            Request.Headers.TryGetValue(AuthSchemeConstants.Scheme, out var encryptedContent);
            _options = JsonConvert.DeserializeObject<ClsAuthSchemeOptions>(encryptedContent.DecryptAes(_apiSetting.Key, _apiSetting.Vector));
            return base.InitializeHandlerAsync();
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (_options is null)
                return Task.FromResult(AuthenticateResult.Fail($"{HttpStatusCode.Unauthorized}"));

            if (_options.CsgwSelectorFlag.IsNullOrEmpty() || _options.CsgwAppId.IsNullOrEmpty() || _options.CsgwUrl.IsNullOrEmpty() || _options.UserId.IsNullOrEmpty())
            {
                return Task.FromResult(AuthenticateResult.Fail($"{HttpStatusCode.Unauthorized}"));
            }
            else
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, $"Test{nameof(EnumSystem.CLS)}");

                var result = AuthenticateResult.Success(ticket);

                return Task.FromResult(result);
            }
        }
    }
}
