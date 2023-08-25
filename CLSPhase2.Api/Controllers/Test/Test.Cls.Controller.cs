using CLSPhase2.Api.Models;
using CLSPhase2.Dal.Entities;
using CLSPhase2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CLSPhase2.Api.Controllers.Test
{
    [Route($"api/{nameof(EnumSystem.Test)}/{nameof(EnumSystem.CLS)}")]
    [ApiExplorerSettings(GroupName = nameof(EnumSystem.Test))]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = nameof(EnumSystem.CLS))]
    [ApiController]
    public class TestClsController : ControllerBase
    {
        private readonly ICSGWService _csgwServices;
        private readonly ICreditLensService _creditLensServices;

        public TestClsController(ICreditLensService creditLensService,
                                 ICSGWService csgwServices)
        {
            _creditLensServices = creditLensService;
            _csgwServices = csgwServices;
        }

        [HttpGet("Secure")]
        public async Task<IActionResult> Secure()
        {
            return Ok();
        }

        [HttpPost("Entity-Information")]
        public async Task<IActionResult> EntityInformation([FromBody] KYCViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);

                var businessPartnerFilter = await _creditLensServices.FilterBusinessPartner(model.entityId, businessPartnerList, EnumSystem.CSGW);

                var entityInfo = await _creditLensServices.RetrieveEntityInformation(businessPartnerFilter.entityInfo, EnumSystem.CSGW);
                return Ok(entityInfo);
            }
            else
                return Unauthorized();
        }

        [HttpPost("Entity-Information/Business-PartnerList")]
        public async Task<IActionResult> BusinessPartnerList([FromBody] KYCViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);
                return Ok(businessPartnerList);
            }
            else
                return Unauthorized();
        }

        [HttpPost("Entity-Information/Business-PartnerList/Aus-NonAus")]
        public async Task<IActionResult> BusinessPartnerListAusNonaus([FromBody] KYCViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);

                var businessPartnerFilter = await _creditLensServices.FilterBusinessPartner(model.entityId, businessPartnerList, EnumSystem.CSGW);

                var entityInfo = await _creditLensServices.RetrieveEntityInformation(businessPartnerFilter.entityInfo, EnumSystem.CSGW);

                var businessPartnerInfo = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CSGW);

                return Ok(businessPartnerInfo.infoAusNonaus);
            }
            else
                return Unauthorized();
        }

        [HttpPost("CSGW/Request-CSGW")]
        public async Task<IActionResult> RequestCSGW([FromBody] KYCViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);

                var businessPartnerFilter = await _creditLensServices.FilterBusinessPartner(model.entityId, businessPartnerList, EnumSystem.CSGW);

                var entityInfo = await _creditLensServices.RetrieveEntityInformation(businessPartnerFilter.entityInfo, EnumSystem.CSGW);

                var businessPartnerInfo = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CSGW);

                var reqCSGW = await _csgwServices.GenerateRequest(businessPartnerInfo.infoAusNonaus);

                return Ok(reqCSGW.calRisk);
            }
            else
                return Unauthorized();
        }

        [HttpPost("CSGW/Calculate-Risk")]
        public async Task<IActionResult> CSGWCalculateRisk([FromBody] KYCViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);

                var businessPartnerFilter = await _creditLensServices.FilterBusinessPartner(model.entityId, businessPartnerList, EnumSystem.CSGW);

                var entityInfo = await _creditLensServices.RetrieveEntityInformation(businessPartnerFilter.entityInfo, EnumSystem.CSGW);

                var businessPartnerInfo = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CSGW);

                var reqCSGW = await _csgwServices.GenerateRequest(businessPartnerInfo.infoAusNonaus);

                var calRisk = await _csgwServices.CalculateRisk(reqCSGW);

                return Ok(calRisk.response);
            }
            else
                return Unauthorized();
        }

        [HttpPost("Calculate-WorstKyc")]
        public async Task<IActionResult> CalculateWorstKyc([FromBody] KYCViewModel model)
        {
            var login = await _creditLensServices.Login();
            if (login != null)
            {
                await _creditLensServices.SetRequestHeader();

                var businessPartnerList = await _creditLensServices.RetrieveBusinessPartnerList(model.entityId);

                var businessPartnerFilter = await _creditLensServices.FilterBusinessPartner(model.entityId, businessPartnerList, EnumSystem.CSGW);

                var entityInfo = await _creditLensServices.RetrieveEntityInformation(businessPartnerFilter.entityInfo, EnumSystem.CSGW);

                var businessPartnerInfo = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CSGW);

                var reqCSGW = await _csgwServices.GenerateRequest(businessPartnerInfo.infoAusNonaus);

                var calRisk = await _csgwServices.CalculateRisk(reqCSGW);
                if (!calRisk.allEntityRisk.Any(a => a.IsSuccess is false))
                    calRisk.allEntityRisk = await _creditLensServices.CalculateWorstKyc(calRisk.allEntityRisk);

                return Ok(calRisk.allEntityRisk);
            }
            else
                return Unauthorized();
        }

        [HttpPost("Request-UpdateCls")]
        public async Task<IActionResult> RequestUpdateCls([FromBody] KYCViewModel model)
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

                var businessPartnerInfo = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CSGW);

                var reqCSGW = await _csgwServices.GenerateRequest(businessPartnerInfo.infoAusNonaus);

                var calRisk = await _csgwServices.CalculateRisk(reqCSGW);
                if (!calRisk.allEntityRisk.Any(a => a.IsSuccess is false))
                    calRisk.allEntityRisk = await _creditLensServices.CalculateWorstKyc(calRisk.allEntityRisk);

                var _resUpdate = await _creditLensServices.UpdateEntityKYCRisk(calRisk.allEntityRisk, EnumMode.Debug);
                return Ok(_resUpdate);
            }
            else
                return Unauthorized();
        }

        [HttpPost("Email-Trigger")]
        public async Task<IActionResult> EmailTrigger([FromBody] KYCViewModel model)
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

                var businessPartnerInfo = await _creditLensServices.RetrieveEntityBusinessPartnerInformation(entityInfo, businessPartnerFilter.partnerList, EnumSystem.CSGW);

                var reqCSGW = await _csgwServices.GenerateRequest(businessPartnerInfo.infoAusNonaus);

                var calRisk = await _csgwServices.CalculateRisk(reqCSGW);

                if (!calRisk.allEntityRisk.Any(a => a.IsSuccess is false))
                    calRisk.allEntityRisk = await _creditLensServices.CalculateWorstKyc(calRisk.allEntityRisk);

                var _resUpdate = await _creditLensServices.UpdateEntityKYCRisk(calRisk.allEntityRisk, EnumMode.Debug);

                return Ok(await _creditLensServices.EntityExecuteRule(_resUpdate, EnumMode.Debug));
            }
            else
                return Unauthorized();
        }
    }
}
