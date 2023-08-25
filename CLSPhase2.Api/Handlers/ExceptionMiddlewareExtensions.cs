using CLSPhase2.Api.Models;
using CLSPhase2.Dal.Entities;
using Microsoft.AspNetCore.Diagnostics;

namespace CLSPhase2.Api.Handler
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ExceptionHandler(this IApplicationBuilder app, ILoggerFactory logger)
        {
            ILogger _appLog = logger.CreateLogger(nameof(EnumLog.AppLogger));
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = context.Response.StatusCode;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var ex = contextFeature.Error;
                        _appLog.LogError($"ERROR {contextFeature.Path} {ex.Message}{ex.StackTrace} {ex.TargetSite}");
                        await context.Response.WriteAsJsonAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = ex.Message
                        });
                    }
                });
            });
        }
    }
}
