using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using CLSPhase2.UnitTests.Factory;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.UnitTests.Controllers
{
    public class BaseControllerTests : IClassFixture<ApiApplicationFactory<Program>>
    {
        protected HttpClient client;

        public BaseControllerTests(ApiApplicationFactory<Program> factory)
        {
            client = factory.CreateClient();
        }
        public BaseControllerTests(ApiApplicationFactory<Program> factory, string secondPartUrl)
        {
            client = factory.CreateClient();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add(AuthSchemeConstants.Scheme, encryptedHeaders);
        }
    }
}
