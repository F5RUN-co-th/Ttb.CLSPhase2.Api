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
    public class ClsAuthHandler : AuthenticationHandler<ClsAuthSchemeOptions>
    {
        ClsAuthSchemeOptions _options;
        private readonly ApiSettings _apiSetting;
        protected readonly IUnitOfWork _unitOfWork;
        public ClsAuthHandler(IOptionsMonitor<ClsAuthSchemeOptions> options,
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
                var _tellerId = _options.UserId.IndexOf("TMBBANK-") >= 0 ? _options.UserId[(_options.UserId.LastIndexOf('-') + 1)..] : "";
                _unitOfWork.baseSystem.UserId = _options.UserId;
                _unitOfWork.baseSystem.AppId = _options.CsgwAppId;
                _unitOfWork.baseSystem.csgwModel = new CsgwModel()
                {
                    TellerId = _tellerId,
                    CSGWUseFlag = _options.CsgwSelectorFlag,
                    CSGWUrl = new Uri(_options.CsgwUrl),
                };
                return Task.FromResult(AuthenticateResult.Success(AuthSchemeConstants.CreateTicket(_tellerId, _unitOfWork.baseSystem.UserId, Scheme.Name)));
            }
        }
    }
}


