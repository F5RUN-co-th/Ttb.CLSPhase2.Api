using CLSPhase2.Dal.Infrastructure;
using CLSPhase2.Dal.Interfaces;
using CLSPhase2.Dal.Repositories;

namespace CLSPhase2.Dal.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private IConnectionFactory _connectionFactory;
        public IBaseSystem baseSystem { get; set; }
        public UnitOfWork(IConnectionFactory connectionFactory, IBaseSystem baseSys)
        {
            _connectionFactory = connectionFactory;
            baseSystem = baseSys;
        }

        public ICPssTempRequestBatchRepository CPssTempRequestBatchRepository => new CPssTempRequestBatchRepository(_connectionFactory, baseSystem);
    }
}
