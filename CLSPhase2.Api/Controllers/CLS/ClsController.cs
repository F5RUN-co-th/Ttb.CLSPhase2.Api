using CLSPhase2.Api.Models;
using CLSPhase2.Dal.Entities;
using CLSPhase2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CLSPhase2.Api.Controllers.CLS
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = nameof(EnumSystem.CLS))]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = nameof(EnumSystem.CLS))]
    [ApiController]
    public class ClsController : ControllerBase
    {
        private readonly ICSGWService _csgwServices;
        private readonly IBackgroundTaskQueue _queue;
        private readonly ICreditLensService _creditLensServices;
        public ClsController(IBackgroundTaskQueue queue,
                             ICreditLensService creditLensService,
                             ICSGWService csgwServices)
        {
            _creditLensServices = creditLensService;
            _csgwServices = csgwServices;
            _queue = queue;
        }

        [HttpPost("KYC")]
        public async Task<IActionResult> KYCCheck([FromBody] KYCViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);

                var businessPartnerFilter = await _creditLensServices.FilterBusinessPartner(model.entityId, businessPartnerList, EnumSystem.CSGW);

                var entityInfo = await _creditLensServices.RetrieveEntityInformation(businessPartnerFilter.entityInfo, EnumSystem.CSGW);
                if (entityInfo is null)
                    return Problem();

                await _queue.QueueBackgroundAsync(async (token) =>
                {
                    var businessPartnerInfo = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CSGW);

                    var reqCSGW = await _csgwServices.GenerateRequest(businessPartnerInfo.infoAusNonaus);

                    var calRisk = await _csgwServices.CalculateRisk(reqCSGW);

                    if (!calRisk.allEntityRisk.Any(a => a.IsSuccess is false))
                        calRisk.allEntityRisk = await _creditLensServices.CalculateWorstKyc(calRisk.allEntityRisk);

                    var _resUpdate = await _creditLensServices.UpdateEntityKYCRisk(calRisk.allEntityRisk, EnumMode.Release);

                    await _creditLensServices.EntityExecuteRule(_resUpdate, EnumMode.Release);
                });

                return Ok();
            }
            else
                return Unauthorized();
        }
    }
}
