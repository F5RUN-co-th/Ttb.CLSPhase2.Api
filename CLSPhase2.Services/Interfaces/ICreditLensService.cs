using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.Entities.CLS;
using CLSPhase2.Dal.Entities.CPSS;

namespace CLSPhase2.Services.Interfaces
{
    public interface ICreditLensService
    {
        Task SetRequestHeader();
        Task<TtbUserLogin> Login();
        Task<IEnumerable<TtbEntityInfomation.TmbBusinessPartnerList>> RetrieveBusinessPartnerList(long entityId);
        Task<List<TtbEntityInfoAusNonAus.TtbEntityInformation>> RetrieveEntityInformation(IEnumerable<TtbBusinessPartnerFilterList> entityInfo, EnumSystem logicSys);
        Task<(IEnumerable<TtbEntityInfoAusNonAus.TtbEntityInformation> entityList, TtbEntityInfoAusNonAus infoAusNonaus)> RetrieveEntityBusinessPartnerInformation(List<TtbEntityInfoAusNonAus.TtbEntityInformation> entityInfoList, IEnumerable<TtbBusinessPartnerFilterList> partnerList, EnumSystem logicSys);
        Task<(IEnumerable<TtbBusinessPartnerFilterList> entityInfo, IEnumerable<TtbBusinessPartnerFilterList> partnerList)> FilterBusinessPartner(long entityId, IEnumerable<TtbEntityInfomation.TmbBusinessPartnerList> businessPartnerList, EnumSystem logicSys);
        Task<List<TtbCalculateWorstKyc.EntityRiskLevel>> CalculateWorstKyc(List<TtbCalculateWorstKyc.EntityRiskLevel> allEntityRisk);
        Task<IEnumerable<TtbUpdateEntityCls>> EntityExecuteRule(List<TtbUpdateEntityCls> resUpdate, EnumMode mode);
        Task<List<TtbUpdateEntityCls>> UpdateEntityKYCRisk(List<TtbCalculateWorstKyc.EntityRiskLevel> worstKycResult, EnumMode mode);
        Task<IEnumerable<TtbUpdateEntityScheme>> GenerateUpdateScheme(IEnumerable<TtbMapEntityIndex> entityMappObj);
        Task<IEnumerable<TtbUpdateEntityCsi>> UpdateEntityCSICode(IEnumerable<TtbUpdateEntityScheme> entityList, EnumMode mode);
    }
}
