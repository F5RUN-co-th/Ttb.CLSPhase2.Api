using CLSPhase2.Api.Extensions;
using CLSPhase2.Api.Handlers.AuthHandlers.Scheme;
using CLSPhase2.Api.Models;
using CLSPhase2.Dal.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CLSPhase2.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = nameof(EnumSystem.Test))]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ApiSettings _apiSetting;

        public SettingController(IOptions<ApiSettings> apisetting)
        {
            _apiSetting = apisetting.Value;
        }

        [HttpGet("Debug/SIT/Generate/Headers/Cpss")]
        public async Task<IActionResult> GenerateCpssHeader()
        {
            var param = new CPssAuthSchemeOptions
            {
                UserId = "Ttb-admin",
                CpssInboundUrl = "http://10.209.26.115:8080/rest/NSLL_SVC_SEARCH_BASIC_CM_RESULT/results.json",
                CpssOutboundUrl = "http://10.209.26.115:8080/rest/NSLL_SVC_SEARCH_BASIC_CM_REQUEST/results.json",
                DbUserName = "clsapi",
                DbPassword = "b",
                CpssUsername = "admin",
                CpssPassword = "admin"
            };
            string encryptedContent = JsonConvert.SerializeObject(param);
            string EncryptedHeaders = Convert.ToBase64String(encryptedContent.EncryptAes(_apiSetting.Key, _apiSetting.Vector));
            return Ok(EncryptedHeaders);
        }

        [HttpGet("Debug/SIT/Generate/Headers/Csgw")]
        public async Task<IActionResult> GenerateCsgwHeader()
        {
            var param = new ClsAuthSchemeOptions
            {
                UserId = "Ttb-admin",
                CsgwUrl = "https://10.209.26.136:9400",
                CsgwAppId = "A0391",
                CsgwSelectorFlag = "AAII"
            };
            string encryptedContent = JsonConvert.SerializeObject(param);
            string EncryptedHeaders = Convert.ToBase64String(encryptedContent.EncryptAes(_apiSetting.Key, _apiSetting.Vector));
            return Ok(EncryptedHeaders);
        }
    }
}
