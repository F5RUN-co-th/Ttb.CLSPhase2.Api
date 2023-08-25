using CLSPhase2.Api.Extensions;
using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using CLSPhase2.Api.Handlers.AuthHandlers.Scheme;
using CLSPhase2.Api.Models;
using CLSPhase2.Dal.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace CLSPhase2.UnitTests.Factory
{
    public class TestCPssAuthHandler : AuthenticationHandler<CPssAuthSchemeOptions>
    {
        CPssAuthSchemeOptions _options;
        private readonly ApiSettings _apiSetting;
        public TestCPssAuthHandler(IOptionsMonitor<CPssAuthSchemeOptions> options,
            IOptions<ApiSettings> apisetting,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _apiSetting = apisetting.Value;
        }
        protected override Task InitializeHandlerAsync()
        {
            Request.Headers.TryGetValue(AuthSchemeConstants.Scheme, out var encryptedContent);
            _options = JsonConvert.DeserializeObject<CPssAuthSchemeOptions>(encryptedContent.DecryptAes(_apiSetting.Key, _apiSetting.Vector));
            return base.InitializeHandlerAsync();
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (_options is null)
                return Task.FromResult(AuthenticateResult.Fail($"{HttpStatusCode.Unauthorized}"));

            if (_options.CpssInboundUrl.IsNullOrEmpty() ||
                _options.CpssOutboundUrl.IsNullOrEmpty() ||
                _options.CpssUsername.IsNullOrEmpty() ||
                _options.CpssPassword.IsNullOrEmpty() ||
                _options.DbUserName.IsNullOrEmpty() ||
                _options.DbPassword.IsNullOrEmpty() ||
                _options.UserId.IsNullOrEmpty())
            {
                return Task.FromResult(AuthenticateResult.Fail($"{HttpStatusCode.Unauthorized}"));
            }
            else
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, $"Test{nameof(EnumSystem.CPSS)}");

                var result = AuthenticateResult.Success(ticket);

                return Task.FromResult(result);
            }
        }
    }
}
