using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using CLSPhase2.UnitTests.Factory;
using System.Net;

namespace CLSPhase2.UnitTests
{
    public class SwaggerTests : IClassFixture<ApiApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;

        public SwaggerTests(ApiApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();
        }

        public static IEnumerable<object[]> AreaSectionTestCases()
        {
            foreach (var section in AuthSchemeConstants.AreaAndSecuritySchemeSection.Select(s => s.Key))
            {
                yield return new object[] {
                    section
                };
            }
        }

        [Theory]
        [MemberData(nameof(AreaSectionTestCases))]
        public async Task SwaggerShouldWork(string areaName)
        {
            var response = await _httpClient.GetAsync($"swagger/{areaName}/swagger.json");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
