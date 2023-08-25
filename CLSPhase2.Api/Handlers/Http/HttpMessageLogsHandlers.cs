using CLSPhase2.Dal.Entities;
using System.Text;

namespace CLSPhase2.Api.Handlers.Http
{
    public class HttpMessageLogsHandlers : DelegatingHandler
    {
        protected readonly ILogger _appLog;

        public HttpMessageLogsHandlers(ILoggerFactory loggerFactory) => _appLog = loggerFactory.CreateLogger(nameof(EnumLog.AppLogger));

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Request(request, EnumPayLoadType.Outbound);
            var response = await base.SendAsync(request, cancellationToken);
            await Response(response, EnumPayLoadType.Inbound);

            return response;
        }

        private async Task Response(HttpResponseMessage response, EnumPayLoadType inbound, string logtype = "PAYLOAD")
        {
            var message = new StringBuilder();
            message.Append($"{new
            {
                request_id = Guid.NewGuid(),
                response.RequestMessage.RequestUri,
                response.Version,
                response.Content.Headers,
                response.RequestMessage.Method
            }}");
            if (response.Content != null)
            {
                message.Append($"{await response.Content.ReadAsStringAsync()}");
            }
            _appLog.LogInformation($"{logtype} {inbound} {message}");
        }

        private async Task Request(HttpRequestMessage request, EnumPayLoadType outbound, string logtype = "PAYLOAD")
        {
            var message = new StringBuilder();
            message.Append($"{new
            {
                request_id = Guid.NewGuid(),
                request.RequestUri,
                request.Version,
                request,
                request.Method
            }}");
            if (request.Content != null)
            {
                message.Append($"{await request.Content.ReadAsStringAsync()}");
            }
            _appLog.LogInformation($"{logtype} {outbound} {message}");
        }
    }
}
