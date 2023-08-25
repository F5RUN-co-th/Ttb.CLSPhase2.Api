using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.Entities.CLS;
using CLSPhase2.Dal.Entities.CPSS;

namespace CLSPhase2.Services.Interfaces
{
    public interface ICPssService
    {
        Task<IEnumerable<TempRequestBatch>> GetAllTempRequestBatch();
        Task<IEnumerable<BasicCmResult.Data>> BasicCMResult(IEnumerable<TempRequestBatch> tempRequestList, EnumMode mode);
        IEnumerable<TtbMapEntityIndex> MappingEntityIndex(IEnumerable<BasicCmResult.Data> data);
        Task<BasicCmRequestBatch> GenerateRequest(IEnumerable<TtbEntityInfoAusNonAus.TtbEntityInformation> entityList, EnumBasicCmSrchPatterns srchPatterns);
        Task<IEnumerable<BasicCmRequestBatch.Data>> BasicCMRequest(BasicCmRequestBatch basicCMRequestList, EnumMode mode);
        Task<TempRequestBatch> CreateTempRequestBatch(TempRequestBatch resultRequest);
        Task<IEnumerable<TempRequestBatch>> CreateTempRequestBatch(IEnumerable<TempRequestBatch> resultRequest);
        Task<bool> DeleteTempRequestBatch(IEnumerable<int> idList);
    }
}
