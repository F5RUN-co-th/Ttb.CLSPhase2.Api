using CLSPhase2.Dal.Infrastructure;
using CLSPhase2.Dal.Interfaces;

namespace CLSPhase2.Dal.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IBaseSystem baseSystem { get; set; }

        public ICPssTempRequestBatchRepository CPssTempRequestBatchRepository { get; }
    }
}
