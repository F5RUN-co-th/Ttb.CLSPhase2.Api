using AutoMapper;
using CLSPhase2.Api.Handlers.AuthHandlers.Constants;
using CLSPhase2.Api.Models;
using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.Entities.CLS;
using CLSPhase2.Dal.Entities.CPSS;
using CLSPhase2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace CLSPhase2.Api.Controllers.Test
{
    [Route($"api/{nameof(EnumSystem.Test)}/{nameof(EnumSystem.CPSS)}")]
    [ApiExplorerSettings(GroupName = nameof(EnumSystem.Test))]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = nameof(EnumSystem.CPSS))]
    [ApiController]
    public class TestCpssController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICPssService _cpssService;
        private readonly ICreditLensService _creditLensServices;
        public TestCpssController(ICreditLensService creditLensService,
                                  IMapper mapper,
                                  ICPssService cpssService)
        {
            _cpssService = cpssService;
            _creditLensServices = creditLensService;
            _mapper = mapper;
        }


        [HttpGet("Secure")]
        public async Task<IActionResult> Secure()
        {
            return Ok();
        }

        [HttpPost("Outbound/BASIC-CM-REQUEST")]
        public async Task<IActionResult> CSITest([FromBody] CSIViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);

                var businessPartnerFilter = await _creditLensServices.FilterBusinessPartner(model.entityId, businessPartnerList, EnumSystem.CPSS);

                var entityInfo = await _creditLensServices.RetrieveEntityInformation(businessPartnerFilter.entityInfo, EnumSystem.CPSS);
                if (entityInfo is null)
                    return Problem();

                var entityList = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CPSS);

                var basicCMRequestList = await _cpssService.GenerateRequest(entityList.entityList, EnumBasicCmSrchPatterns.AddressFlag);

                var basicCMResponseList = await _cpssService.BasicCMRequest(basicCMRequestList, EnumMode.Debug);
                if (!basicCMResponseList.Any(a => a.IsSuccess is false))
                {
                    var requestBatchList = basicCMResponseList.Select(s => new TempRequestBatch
                    {
                        ReferenceCode = s.response.Output.FirstOrDefault().referenceCode ?? "",
                        JsonDocOutboundRequest = s.outbound,
                        JsonDocOutboundResponse = JsonConvert.SerializeObject(s.response)
                    });
                    var result = await _cpssService.CreateTempRequestBatch(requestBatchList);
                    return Ok(result);
                }
                else
                    return Problem();
            }
            else
                return Unauthorized();
        }

        [HttpPost("Generate/Outbound-Request")]
        public async Task<IActionResult> GenerateCSIRequest([FromBody] CSIViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);

                var businessPartnerFilter = await _creditLensServices.FilterBusinessPartner(model.entityId, businessPartnerList, EnumSystem.CPSS);

                var entityInfo = await _creditLensServices.RetrieveEntityInformation(businessPartnerFilter.entityInfo, EnumSystem.CPSS);
                if (entityInfo is null)
                    return Problem();

                var entityList = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CPSS);

                var basicCMRequestList = await _cpssService.GenerateRequest(entityList.entityList, EnumBasicCmSrchPatterns.AddressFlag);

                return Ok(basicCMRequestList.data.Select(s => s.request));
            }
            else
                return Unauthorized();
        }

        [HttpPost("Generate/Outbound-Request/Non-AddressFlag")]
        public async Task<IActionResult> GenerateCSIReqNonAddressFlag([FromBody] CSIViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);

                var businessPartnerFilter = await _creditLensServices.FilterBusinessPartner(model.entityId, businessPartnerList, EnumSystem.CPSS);

                var entityInfo = await _creditLensServices.RetrieveEntityInformation(businessPartnerFilter.entityInfo, EnumSystem.CPSS);
                if (entityInfo is null)
                    return Problem();

                var entityList = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CPSS);

                var basicCMRequestList = await _cpssService.GenerateRequest(entityList.entityList, EnumBasicCmSrchPatterns.NonAddressFlag);

                return Ok(basicCMRequestList.data.Select(s => JsonConvert.DeserializeObject(s.request)));
            }
            else
                return Unauthorized();
        }

        [HttpGet("Inbound/BASIC-CM-Result")]
        public async Task<IActionResult> GetCSIResultTest()
        {
            var tempRequests = await _cpssService.GetAllTempRequestBatch();

            var basicCMResultList = await _cpssService.BasicCMResult(tempRequests, EnumMode.Release);
            if (!basicCMResultList.Any(a => a.IsSuccess is false))
            {
                var mappEntityList = _cpssService.MappingEntityIndex(basicCMResultList);
                var login = await _creditLensServices.Login();
                if (login != null)
                {
                    await _creditLensServices.SetRequestHeader();
                    var updateEntityScheme = await _creditLensServices.GenerateUpdateScheme(mappEntityList);
                    var updateResultList = await _creditLensServices.UpdateEntityCSICode(updateEntityScheme, EnumMode.Debug);
                    return Ok(updateResultList);
                }
                else
                    return Unauthorized();
            }
            else
                return Problem();
        }

        [HttpGet("Inbound/BASIC-CM-Result/Mapping-Entity")]
        public async Task<IActionResult> GetCSIResultMappingEntityIndexTest()
        {
            var tempRequests = await _cpssService.GetAllTempRequestBatch();

            var basicCMResultList = await _cpssService.BasicCMResult(tempRequests, EnumMode.Release);
            if (!basicCMResultList.Any(a => a.IsSuccess is false))
            {
                var mappEntityList = _cpssService.MappingEntityIndex(basicCMResultList);
                return Ok(mappEntityList);
            }
            else
                return Problem();
        }

        [HttpGet("Inbound/BASIC-CM-Result/Generate-UpdateScheme")]
        public async Task<IActionResult> GetCSIResultGenerateUpdateSchemeTest()
        {
            var tempRequests = await _cpssService.GetAllTempRequestBatch();

            var basicCMResultList = await _cpssService.BasicCMResult(tempRequests, EnumMode.Release);
            if (!basicCMResultList.Any(a => a.IsSuccess is false))
            {
                var mappEntityList = _cpssService.MappingEntityIndex(basicCMResultList);
                var login = await _creditLensServices.Login();
                if (login != null)
                {
                    await _creditLensServices.SetRequestHeader();
                    var updateEntityScheme = await _creditLensServices.GenerateUpdateScheme(mappEntityList);
                    return Ok(updateEntityScheme);
                }
                else
                    return Unauthorized();
            }
            else
                return Problem();
        }

        [HttpDelete("Inbound/Delete/All/TempRequest")]
        public async Task<IActionResult> DeleteAllTempRequestTest()
        {
            var tempRequests = await _cpssService.GetAllTempRequestBatch();
            await _cpssService.DeleteTempRequestBatch(tempRequests.Select(s => s.Id));
            return Ok();
        }

        [HttpPost("Debug/Local/Remote/Outbound")]
        public async Task<IActionResult> DebugRemoteOutbound([FromBody] CSIViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);

                var businessPartnerFilter = await _creditLensServices.FilterBusinessPartner(model.entityId, businessPartnerList, EnumSystem.CPSS);

                var entityInfo = await _creditLensServices.RetrieveEntityInformation(businessPartnerFilter.entityInfo, EnumSystem.CPSS);
                if (entityInfo is null)
                    return Problem();

                var entityList = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CPSS);

                var basicCMRequestList = await _cpssService.GenerateRequest(entityList.entityList, EnumBasicCmSrchPatterns.AddressFlag);

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (var apiServer = new HttpClient(handler))
                {
                    apiServer.BaseAddress = new Uri("https://172.28.44.168/");
                    apiServer.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    apiServer.DefaultRequestHeaders.Add(AuthSchemeConstants.Scheme, Request.Headers[AuthSchemeConstants.Scheme].ToString());
                    var result = await apiServer.PostAsync("/api/cpss/debug/remote/csi/outbound", new StringContent(JsonConvert.SerializeObject(basicCMRequestList), Encoding.UTF8, "application/json"));
                    var content = await result.Content.ReadAsStringAsync();
                    return Ok(JsonConvert.DeserializeObject<IEnumerable<TempRequestBatch>>(content));
                }
            }
            else
                return Unauthorized();
        }

        [HttpGet("Debug/Local/Remote/Inbound")]
        public async Task<IActionResult> DebugRemoteInbound()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            using (var apiServer = new HttpClient(handler))
            {
                apiServer.BaseAddress = new Uri("https://172.28.44.168/");
                apiServer.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                apiServer.DefaultRequestHeaders.Add(AuthSchemeConstants.Scheme, Request.Headers[AuthSchemeConstants.Scheme].ToString());
                var result = await apiServer.GetAsync("/api/cpss/debug/remote/csi/inbound");
                var content = await result.Content.ReadAsStringAsync();
                var basicCMResultList = JsonConvert.DeserializeObject<IEnumerable<BasicCmResult.Data>>(content);
                if (!basicCMResultList.Any(a => a.IsSuccess is false))
                {
                    var mappEntityList = _cpssService.MappingEntityIndex(basicCMResultList);
                    var login = await _creditLensServices.Login();
                    if (login != null)
                    {
                        await _creditLensServices.SetRequestHeader();
                        var updateEntityScheme = await _creditLensServices.GenerateUpdateScheme(mappEntityList);
                        var updateResultList = await _creditLensServices.UpdateEntityCSICode(updateEntityScheme, EnumMode.Debug);
                        return Ok(updateResultList);
                    }
                    return Ok();
                }
                else
                    return Problem();
            }
        }

        [HttpGet("Debug/Outbound")]
        public async Task<IActionResult> DebugOutbound()
        {
            var partnersList = new List<TtbEntityInfoAusNonAus.TtbEntityInformation>();
            for (int i = 1; i < 40; i++)
            {
                var obj = new TtbEntityInfoAusNonAus.TtbEntityInformation
                {
                    EntityId = new Random().Next(1000, 9999),
                    EntityType = i > 20 ? "C" : "P",
                    PartnerType = i > 20 ? $"AUS0{i}" : $"AUT0{i}",
                    TmbCustomerId = i + "",
                    TmbBorrowerNameTh = $"bc{i}",
                    TmbBorrowerSurnameTh = $"bc{i}",
                    TmbBorrowerFullNameTh = $"bc{i}",
                    TmbBorrowerFullNameEn = $"bc{i}",
                    TmbIdentificationId = $"bc{i}",
                    TmbBusinessCode = $"bc{i}",
                    Address = new List<TtbEntityInfoAusNonAus.Address>()
                    {
                       new TtbEntityInfoAusNonAus.Address { TmbAddressType = new List<string>() { "REG", "PRI", "OFF","A","B","C","X","Y","Z","" } }
                    }
                };
                partnersList.Add(obj);
            }
            var basicCMRequestList = await _cpssService.GenerateRequest(partnersList, EnumBasicCmSrchPatterns.AddressFlag);

            var basicCMResponseList = await _cpssService.BasicCMRequest(basicCMRequestList, EnumMode.Debug);
            if (!basicCMResponseList.Any(a => a.IsSuccess is false))
            {
                var requestBatchList = basicCMResponseList.Select(s => new TempRequestBatch
                {
                    ReferenceCode = s.response.Output.FirstOrDefault().referenceCode ?? "",
                    JsonDocOutboundRequest = s.outbound,
                    JsonDocOutboundResponse = JsonConvert.SerializeObject(s.response)
                });
                var result = await _cpssService.CreateTempRequestBatch(requestBatchList);
                return Ok(result);
            }
            else
                return Problem();
        }

        [HttpGet("Debug/Inbound")]
        public async Task<IActionResult> DebugInbound()
        {
            var tempRequests = await _cpssService.GetAllTempRequestBatch();

            var basicCMResultList = await _cpssService.BasicCMResult(tempRequests, EnumMode.Debug);
            if (!basicCMResultList.Any(a => a.IsSuccess is false))
            {
                var mappEntityList = _cpssService.MappingEntityIndex(basicCMResultList);
                var updateEntityScheme = await _creditLensServices.GenerateUpdateScheme(mappEntityList);
                return Ok(updateEntityScheme);
            }
            else
                return Problem();
        }
    }
}