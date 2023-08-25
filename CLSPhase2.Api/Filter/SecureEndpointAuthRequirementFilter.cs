using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;

namespace CLSPhase2.Api.Filter
{
    public class SecureEndpointAuthRequirementFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var SecurityRequirementList = new List<OpenApiSecurityRequirement>();
            var areaReqSectionByDocument = AuthSchemeConstants.ApiSecurReqByEndpoint.Where(w => w.Key == context.DocumentName).SelectMany(s => s.Value);
            operation.Responses.Add($"{(int)HttpStatusCode.Unauthorized}", new OpenApiResponse { Description = $"{HttpStatusCode.Unauthorized}" });
            operation.Responses.Add($"{(int)HttpStatusCode.InternalServerError}", new OpenApiResponse { Description = $"{HttpStatusCode.InternalServerError}" });
            foreach (var reqSection in areaReqSectionByDocument)
            {
                SecurityRequirementList.Add(new OpenApiSecurityRequirement()
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = reqSection
                                        },
                                        In = ParameterLocation.Header
                                    }, new string[]{ }
                                }
                            });
            }
            operation.Security = SecurityRequirementList;
        }
    }
}
