using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using CLSPhase2.UnitTests.Factory;
using Microsoft.AspNetCore.Hosting.Server;
using CLSPhase2.Dal.Entities;
using CLSPhase2.Api.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.PortableExecutable;
using CLSPhase2.Api.Handlers.AuthHandlers.Scheme;
using System.Runtime.InteropServices;

namespace CLSPhase2.UnitTests.Auth
{
    public class AuthenticationTests : IClassFixture<ApiApplicationFactory<Program>>
    {
        private readonly ApiApplicationFactory<Program> _factory;

        public AuthenticationTests(ApiApplicationFactory<Program> factory)
        {
            _factory = factory;

        }

        [Fact]
        public async Task CSGWAuthenticatedUserShouldBeOK()
        {
            var resHeader = await _factory.CreateClient().GetAsync("api/setting/debug/sit/generate/headers/csgw");
            
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication($"Test{nameof(EnumSystem.CLS)}")
                        .AddScheme<ClsAuthSchemeOptions, TestClsAuthHandler>(
                            $"Test{nameof(EnumSystem.CLS)}", options => { });
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Test{nameof(EnumSystem.CLS)}");

            client.DefaultRequestHeaders.Add(AuthSchemeConstants.Scheme, await resHeader.Content.ReadAsStringAsync());

            var response = await client.GetAsync($"api/{nameof(EnumSystem.Test)}/{nameof(EnumSystem.CLS)}/Secure".ToLower());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CPSSAuthenticatedUserShouldBeOK()
        {
            var resHeader = await _factory.CreateClient().GetAsync("api/setting/debug/sit/generate/headers/cpss");

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication($"Test{nameof(EnumSystem.CPSS)}")
                        .AddScheme<CPssAuthSchemeOptions, TestCPssAuthHandler>(
                            $"Test{nameof(EnumSystem.CPSS)}", options => { });
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Test{nameof(EnumSystem.CPSS)}");

            client.DefaultRequestHeaders.Add(AuthSchemeConstants.Scheme, await resHeader.Content.ReadAsStringAsync());

            var response = await client.GetAsync($"api/{nameof(EnumSystem.Test)}/{nameof(EnumSystem.CPSS)}/Secure".ToLower());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
