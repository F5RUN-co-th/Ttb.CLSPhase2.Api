using Microsoft.AspNetCore.Authentication;

namespace CLSPhase2.Api.Handlers.AuthHandlers.Scheme
{
    public class ClsAuthSchemeOptions : AuthenticationSchemeOptions
    {
        public string UserId { get; set; }
        public string CsgwUrl { get; set; }
        public string CsgwAppId { get; set; }
        public string CsgwSelectorFlag { get; set; }
    }
}
