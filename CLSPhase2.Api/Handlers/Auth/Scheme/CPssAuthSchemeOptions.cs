using Microsoft.AspNetCore.Authentication;

namespace CLSPhase2.Api.Handlers.AuthHandlers.Scheme
{
    public class CPssAuthSchemeOptions : AuthenticationSchemeOptions
    {
        public string UserId { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
        public string CpssInboundUrl { get; set; }
        public string CpssOutboundUrl { get; set; }
        public string CpssUsername { get; set; }
        public string CpssPassword { get; set; }
    }
}
