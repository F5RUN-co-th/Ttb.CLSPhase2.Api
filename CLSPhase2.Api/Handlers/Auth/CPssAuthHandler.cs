using CLSPhase2.Api.Extensions;
using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using CLSPhase2.Api.Handlers.AuthHandlers.Scheme;
using CLSPhase2.Api.Models;
using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.UnitOfWork;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using System.Text.Encodings.Web;

namespace CLSPhase2.Api.Handlers.AuthHandlers
{
    public class CPssAuthHandler : AuthenticationHandler<CPssAuthSchemeOptions>
    {
        CPssAuthSchemeOptions _options;
        private readonly ApiSettings _apiSetting;
        protected readonly IUnitOfWork _unitOfWork;
        public CPssAuthHandler(IOptionsMonitor<CPssAuthSchemeOptions> options,
                               IOptions<ApiSettings> apisetting,
                               ILoggerFactory logger,
                               IUnitOfWork unitOfWork,
                               UrlEncoder encoder,
                               ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _unitOfWork = unitOfWork;
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
                _unitOfWork.baseSystem.UserId = _options.UserId;
                _unitOfWork.baseSystem.AppId = "A0391";
                _unitOfWork.baseSystem.cpssModel = new CpssModel()
                {
                    CpssUsername = _options.CpssUsername,
                    CpssPassword = _options.CpssPassword,
                    DbUserName = _options.DbUserName,
                    DbPassword = _options.DbPassword,
                    CpssInboundUrl = new Uri(_options.CpssInboundUrl),
                    CpssOutboundUrl = new Uri(_options.CpssOutboundUrl),
                };
                return Task.FromResult(AuthenticateResult.Success(AuthSchemeConstants.CreateTicket($"{_unitOfWork.baseSystem.UserId}{_unitOfWork.baseSystem.AppId}", _unitOfWork.baseSystem.UserId, Scheme.Name)));
            }
        }
    }
}