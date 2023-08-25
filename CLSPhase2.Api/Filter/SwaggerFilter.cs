using CLSPhase2.Dal.Entities;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace CLSPhase2.Api.Filter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SwaggerAttribute : Attribute
    {
    }
    public class SwaggerFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var _Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
            if (_Environment.Equals(nameof(EnumEnvironments.Local), StringComparison.OrdinalIgnoreCase) || _Environment.Equals(nameof(EnumEnvironments.Development), StringComparison.OrdinalIgnoreCase))
            {

            }
            else
            {
                var sitLists = swaggerDoc.Paths.Where(pathItem => !pathItem.Key.Contains(nameof(EnumEnvironments.SIT).ToLower()) && pathItem.Key.Contains(context.DocumentName.ToLower()));
                if (_Environment.Equals(nameof(EnumEnvironments.SIT), StringComparison.OrdinalIgnoreCase) && sitLists.Any())
                {
                    foreach (var d in sitLists.Where(pathItem => pathItem.Key.Contains(nameof(EnumMode.Debug).ToLower())))
                        swaggerDoc.Paths.Remove(d.Key);
                }
                else
                {
                    var debugLists = swaggerDoc.Paths.Where(pathItem => pathItem.Key.Contains(nameof(EnumMode.Debug).ToLower()));
                    foreach (var d in debugLists)
                        swaggerDoc.Paths.Remove(d.Key);
                }
            }
        }
    }
}
