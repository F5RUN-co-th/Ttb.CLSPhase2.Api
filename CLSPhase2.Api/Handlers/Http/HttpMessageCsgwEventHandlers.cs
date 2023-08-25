using CLSPhase2.Dal.Entities;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;

namespace CLSPhase2.Api.Handlers.Http
{
    public class HttpMessageCsgwEventHandlers : DelegatingHandler
    {
        protected readonly ILogger _appEventLog;

        public HttpMessageCsgwEventHandlers(ILoggerFactory loggerFactory) => _appEventLog = loggerFactory.CreateLogger(nameof(EnumLog.AppEventLogger));

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var timmer = new Stopwatch();
            timmer.Start();
            var response = await base.SendAsync(request, cancellationToken);
            timmer.Stop();

            await Event(request, response, timmer);

            return response;
        }

        private async Task Event(HttpRequestMessage request, HttpResponseMessage response, Stopwatch timmer, string logtype = "EVENT")
        {
            var reqMessage = await request.Content.ReadAsStringAsync();
            var resMessage = await response.Content.ReadAsStringAsync();
            var message = new StringBuilder();
            if (!response.IsSuccessStatusCode)
            {
                message.Append($"{new
                {
                    event_type = "BIZ-ERROR”",
                    status = "FAIL",
                    parameters = new
                    {
                        error_code = (int)response.StatusCode,
                        error_name = response.ReasonPhrase,
                    },
                    http_code = (int)response.StatusCode,
                    response_time = timmer.ElapsedMilliseconds,
                    uri = request.RequestUri
                }}");
                _appEventLog.LogWarning($"{logtype} {message}");
            }
            else
            {
                if (string.IsNullOrEmpty(resMessage))
                {
                    message.Append($"{new
                    {
                        event_type = "BIZ-ERROR”",
                        status = "FAIL",
                        parameters = new
                        {
                            error_code = (int)response.StatusCode,
                            error_name = response.ReasonPhrase,
                            request_body = reqMessage
                        },
                        http_code = (int)response.StatusCode,
                        response_time = timmer.ElapsedMilliseconds,
                        uri = request.RequestUri
                    }}");
                    _appEventLog.LogWarning($"{logtype} {message}");
                }
                else
                {
                    var jObjContent = JObject.Parse(resMessage);
                    var jObjResultStatusCode = jObjContent["header"]["resCode"].ToString();
                    if (EnumCSGWCalRiskResCode.Success == (EnumCSGWCalRiskResCode)Enum.Parse(typeof(EnumCSGWCalRiskResCode), jObjResultStatusCode))
                    {
                        foreach (var @event in Enum.GetNames(typeof(EnumEventType)))
                        {
                            message = new StringBuilder();
                            if (@event == $"{EnumEventType.OPERATION}" || @event == $"{EnumEventType.BUSINESS}")
                            {
                                message.Append($"{new
                                {
                                    event_type = @event,
                                    status = @event == $"{EnumEventType.BUSINESS}" ? "SUCCESS" : response.StatusCode == System.Net.HttpStatusCode.OK ? "SUCCESS" : "FAIL",
                                    parameters = reqMessage,
                                    http_code = (int)response.StatusCode,
                                    response_time = timmer.ElapsedMilliseconds,
                                    uri = request.RequestUri
                                }}");
                                _appEventLog.LogInformation($"{logtype} {@event} {message}");
                            }
                        }
                    }
                    else
                    {
                        message.Append($"{new
                        {
                            event_type = "BIZ-ERROR”",
                            status = "FAIL",
                            parameters = new
                            {
                                error_code = jObjResultStatusCode,
                                error_name = jObjContent["header"]["resDesc"] ?? "",
                                request_body = reqMessage
                            },
                            http_code = (int)response.StatusCode,
                            response_time = timmer.ElapsedMilliseconds,
                            uri = request.RequestUri
                        }}");
                        _appEventLog.LogWarning($"{logtype} {message}");
                    }
                }
            }
        }
    }
}
