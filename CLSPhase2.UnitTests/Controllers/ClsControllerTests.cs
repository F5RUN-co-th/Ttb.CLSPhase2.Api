using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using CLSPhase2.Api.Models;
using CLSPhase2.UnitTests.Factory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CLSPhase2.UnitTests.Controllers
{
    public class ClsControllerTests : BaseControllerTests
    {
        public ClsControllerTests(ApiApplicationFactory<Program> factory)
            : base(factory, "api/setting/debug/sit/generate/headers/csgw")
        {

        }

        [Theory]
        [InlineData(55258)]
        public async Task POST_ReturnSuccess(long entityId)
        {
            var resHeader = await this.client.GetAsync("api/setting/debug/sit/generate/headers/csgw");

            client.DefaultRequestHeaders.Add(AuthSchemeConstants.Scheme, await resHeader.Content.ReadAsStringAsync());

            var _body = new KYCViewModel { entityId = entityId };

            var response = await this.client.PostAsync("api/cls/kyc", new StringContent(JsonConvert.SerializeObject(_body), Encoding.UTF8, "application/json"));
            
            var responseStatusCode = response.StatusCode;
            
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, responseStatusCode);
        }
    }
}
