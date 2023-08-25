using CLSPhase2.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Interfaces
{
    public interface ICPssTempRequestBatchRepository : IGenericRepository<TempRequestBatch>
    {
        Task<int> Create(Dictionary<string, object> dataStore);
        Task<int> Create(Dictionary<string[], IEnumerable<Dictionary<string, object>>> dataStore);
        Task<bool> Delete(object[] idList);
        Task<IEnumerable<TempRequestBatch>> GetTempRequestBatchByItemList(Dictionary<string[], IEnumerable<Dictionary<string, object>>> dataStore);

    }
}
