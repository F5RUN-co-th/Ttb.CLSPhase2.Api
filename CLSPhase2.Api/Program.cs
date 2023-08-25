using CLSPhase2.Api;
using CLSPhase2.Api.Extensions;
using CLSPhase2.Api.Filter;
using CLSPhase2.Api.Handler;
using CLSPhase2.Api.Handlers.AuthHandlers;
using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using CLSPhase2.Api.Handlers.AuthHandlers.Scheme;
using CLSPhase2.Api.Handlers.Http;
using CLSPhase2.Api.Models;
using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.Infrastructure;
using CLSPhase2.Dal.UnitOfWork;
using CLSPhase2.Services.Interfaces;
using CLSPhase2.Services.Services;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Polly;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
string _Environment;
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    {
        var services = builder.Services;

        var configuration = builder.Configuration;
        var env = builder.Environment;

        builder.Logging.ClearProviders();

        builder.Host.UseNLog();

        _Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
        configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{_Environment}.json", false, true);

        var apiSettingsSection = configuration.GetSection(nameof(ApiSettings));

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        services.Configure<ApiSettings>(apiSettingsSection);

        services.AddControllers();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SecureEndpointAuthRequirementFilter>();

            foreach (var areaSection in AuthSchemeConstants.AreaAndSecuritySchemeSection)
            {
                c.SwaggerDoc($"{areaSection.Key}", new OpenApiInfo { Title = $"{areaSection.Key}" });

                foreach (var securScheme in areaSection.Value)
                {
                    c.AddSecurityDefinition(securScheme, new OpenApiSecurityScheme
                    {
                        Name = securScheme,
                        Scheme = "ApiKeyScheme",
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Description = $"`Please enter the {securScheme} for authorization`",
                    });
                }
            }
            c.DocumentFilter<SwaggerFilter>();
        });

        services.AddAuthentication();

        services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.PropertyNamingPolicy = null;

            x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        services.AddScoped<IBaseSystem, BaseSystem>();

        services.AddScoped<IConnectionFactory, ConnectionFactory>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICreditLensService, CreditLensService>();

        services.AddScoped<ICSGWService, CSGWService>();

        services.AddScoped<ICPssService, CPssService>();

        services.AddTransient<HttpMessageLogsHandlers>();

        services.AddTransient<HttpMessageCsgwEventHandlers>();

        services.AddTransient<HttpMessageCpssEventHandlers>();

        services.AddHttpClient<ICreditLensService, CreditLensService>((provider, httpClient) =>
        {
            httpClient.BaseAddress = new Uri(apiSettingsSection["CreditLenSettings:Url"] ?? "");

        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            if (_Environment.Equals(nameof(EnumEnvironments.Local), StringComparison.OrdinalIgnoreCase))
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            return handler;

        }).AddHttpMessageHandler<HttpMessageLogsHandlers>();

        IAsyncPolicy<HttpResponseMessage> waitAndRetryPolicy = Policy.Handle<HttpRequestException>()
                                                                        .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.RequestTimeout)
                                                                            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * 1), OnRetry);
        IAsyncPolicy<HttpResponseMessage> timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMinutes(5));

        services.AddHttpClient<ICSGWService, CSGWService>((provider, httpClient) =>
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            //if (_Environment.Equals(nameof(EnumEnvironments.Local), StringComparison.OrdinalIgnoreCase))
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            return handler;
        }).SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(Policy.WrapAsync(waitAndRetryPolicy, timeoutPolicy))
                .AddHttpMessageHandler<HttpMessageCsgwEventHandlers>();

        services.AddHttpClient<ICPssService, CPssService>((provider, httpClient) =>
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }).AddHttpMessageHandler<HttpMessageCpssEventHandlers>();

        services.AddAuthentication()
                    .AddScheme<ClsAuthSchemeOptions, ClsAuthHandler>(nameof(EnumSystem.CLS), options => { })
                    .AddScheme<CPssAuthSchemeOptions, CPssAuthHandler>(nameof(EnumSystem.CPSS), options => { });

        services.AddSqlMapperExtensions(apiSettingsSection);

        services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddHostedService<QueuedHostedService>();

        builder.Services.AddSingleton<IBackgroundTaskQueue>(_ =>
        {
            if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity))
                queueCapacity = 100;

            return new BackgroundTaskQueue(queueCapacity);
        });
    }
    var app = builder.Build();

    var hostingEnv = app.Services.GetRequiredService<IWebHostEnvironment>();

    app.ExceptionHandler(app.Services.GetService<ILoggerFactory>());

    if (!_Environment.Equals(nameof(EnumEnvironments.UAT), StringComparison.OrdinalIgnoreCase))
    {
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            foreach (var areaSection in AuthSchemeConstants.AreaAndSecuritySchemeSection)
                c.SwaggerEndpoint($"/swagger/{areaSection.Key}/swagger.json", $"{areaSection.Key}");

#if DEBUG
            if (!_Environment.Equals(nameof(EnumEnvironments.UAT), StringComparison.OrdinalIgnoreCase))
            {
                app.UseReDoc(options =>
                {
                    foreach (var areaSection in AuthSchemeConstants.AreaAndSecuritySchemeSection)
                    {
                        options.DocumentTitle = $"{areaSection.Key}";
                        options.SpecUrl = $"/swagger/{areaSection.Key}/swagger.json";
                    }
                });
            }
            else
            {
                c.RoutePrefix = string.Empty;
            }
#else
            c.RoutePrefix = string.Empty;
#endif

        });
    }

    app.UseStatusCodePages();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseAuthentication();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
Task OnRetry(DelegateResult<HttpResponseMessage> response, TimeSpan span, int retryCount, Context context)
{
    if (response == null)
        return Task.CompletedTask;

    var responseResult = response.Result;
    logger.Info($"RetryCount={retryCount} IsSuccess={(responseResult == null ? "" : responseResult.IsSuccessStatusCode)} StatusCode={(responseResult == null ? "" : responseResult.StatusCode)} Exception={response.Exception?.Message}");

    response.Result?.Dispose();
    return Task.CompletedTask;
}
public partial class Program { }








































































































































































































































































#region .bak
//services.AddHttpClient(typeof(CSGWService).Name, (provider, httpClient) =>
//{
//    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//}).ConfigurePrimaryHttpMessageHandler(() =>
//{
//    var handler = new HttpClientHandler();
//    //if (hostingEnvironment.IsDevelopment())
//    //{
//    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
//    //}
//    return handler;
//}).AddHttpMessageHandler<HttpMessageEventHandlers>();

//.SetHandlerLifetime(TimeSpan.FromMinutes(5))
//.AddTransientHttpErrorPolicy(p =>
//    p.OrResult(response => response.StatusCode == HttpStatusCode.RequestTimeout)
//     .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * 1)))
#endregion