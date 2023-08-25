using CLSPhase2.Dal.Entities;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace CLSPhase2.Api.Handlers.AuthHandlers.Constants
{
    public class AuthSchemeConstants
    {
        private static string _Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
        public static readonly string Scheme = "EncryptedHeaders";
        public static Dictionary<string, string[]> AreaAndSecuritySchemeSection { get; } = _Environment.Equals(nameof(EnumEnvironments.UAT), StringComparison.OrdinalIgnoreCase) ?
            new Dictionary<string, string[]>
        {
            { nameof(EnumSystem.CLS), new []{ Scheme }},
            { nameof(EnumSystem.CPSS), Array.Empty<string>() }
        } : new Dictionary<string, string[]>
        {
            { nameof(EnumSystem.CLS), new []{ Scheme }},
            { nameof(EnumSystem.CPSS), Array.Empty<string>() },
            { nameof(EnumSystem.Test), Array.Empty<string>() }
        };
        public static Dictionary<string, string[]> ApiSecurReqByEndpoint { get; } = _Environment.Equals(nameof(EnumEnvironments.UAT), StringComparison.OrdinalIgnoreCase) ?
            new Dictionary<string, string[]>
        {
            { nameof(EnumSystem.CLS), new [] { Scheme }},
            { nameof(EnumSystem.CPSS), new [] { Scheme } },
        } : new Dictionary<string, string[]>
        {
            { nameof(EnumSystem.CLS), new [] { Scheme }},
            { nameof(EnumSystem.CPSS), new [] { Scheme } },
            { nameof(EnumSystem.Test), new [] { Scheme } }
        };

        internal static AuthenticationTicket CreateTicket(string tellerId, string userId, string authenticationScheme)
        {
            var claimsIdentity = new ClaimsIdentity((Claim[]?)(new[] {
                        new Claim(ClaimTypes.NameIdentifier, tellerId),
                        new Claim(ClaimTypes.Name, userId) }), nameof(ClsAuthHandler));
            claimsIdentity.AddClaim(new Claim("IssueDate", DateTime.Now.ToString("yyyyMMddTHHmmssfffffffK")));
            return new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), authenticationScheme);
        }
    }
}
