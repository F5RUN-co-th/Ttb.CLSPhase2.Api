using CLSPhase2.Api.Models;
using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.Entities.CPSS;
using CLSPhase2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CLSPhase2.Api.Controllers.CPSS
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = nameof(EnumSystem.CPSS))]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = nameof(EnumSystem.CPSS))]
    [ApiController]
    public class CpssController : ControllerBase
    {
        private readonly ICPssService _cpssService;
        private readonly ICreditLensService _creditLensServices;
        public CpssController(ICreditLensService creditLensService,
                              ICPssService cpssService)
        {
            _cpssService = cpssService;
            _creditLensServices = creditLensService;
        }

        [HttpPost("CSI")]
        public async Task<IActionResult> CSICheck([FromBody] CSIViewModel model)
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

                var basicCMResponseList = await _cpssService.BasicCMRequest(basicCMRequestList, EnumMode.Release);
                if (!basicCMResponseList.Any(a => a.IsSuccess is false))
                {
                    var requestBatchList = basicCMResponseList.Select(s => new TempRequestBatch
                    {
                        ReferenceCode = s.response.Output.FirstOrDefault().referenceCode ?? "",
                        JsonDocOutboundRequest = s.outbound,
                        JsonDocOutboundResponse = JsonConvert.SerializeObject(s.response)
                    });
                    var result = await _cpssService.CreateTempRequestBatch(requestBatchList);
                    return Ok();
                }
                else
                    return Problem();
            }
            else
                return Unauthorized();
        }

        [HttpPost("CSI/Result")]
        public async Task<IActionResult> CSIResult()
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
                    updateResultList = updateResultList.Where(a => a.IsSuccess is true).DistinctBy(d => d.updateScheme.Id);
                    if (updateResultList.Any())
                    {
                        await _cpssService.DeleteTempRequestBatch(updateResultList.Select(s => s.updateScheme.Id));
                    }
                    return Ok();
                }
                else
                    return Unauthorized();
            }
            return Ok();
        }

        [HttpPost("Debug/Remote/CSI/Outbound")]
        public async Task<IActionResult> Debug([FromBody] BasicCmRequestBatch basicCMRequestList)
        {
            var basicCMResponseList = await _cpssService.BasicCMRequest(basicCMRequestList, EnumMode.Release);
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

        [HttpGet("Debug/Remote/CSI/Inbound")]
        public async Task<IActionResult> Debug()
        {
            var tempRequests = await _cpssService.GetAllTempRequestBatch();

            return Ok(await _cpssService.BasicCMResult(tempRequests, EnumMode.Release));
        }
    }
}
