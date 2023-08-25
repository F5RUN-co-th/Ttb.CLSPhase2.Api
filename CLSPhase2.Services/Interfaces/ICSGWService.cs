using CLSPhase2.Dal.Entities.CLS;
using CLSPhase2.Dal.Entities.CSGW;

namespace CLSPhase2.Services.Interfaces
{
    public interface ICSGWService
    {
        Task<TtbRequestCSGW> GenerateRequest(TtbEntityInfoAusNonAus infoAusNonaus);
        Task<TtbCalculateRisk> CalculateRisk(TtbRequestCSGW obj);
    }
}
