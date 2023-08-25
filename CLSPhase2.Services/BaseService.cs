using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.Infrastructure;
using CLSPhase2.Dal.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace CLSPhase2.Services
{
    public abstract class BaseService
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected readonly IBaseSystem _baseSys;

        protected readonly ILogger _appLog;
        public BaseService(IUnitOfWork unitOfWork, ILoggerFactory loggerFactory)
        {
            _unitOfWork = unitOfWork;
            _baseSys = unitOfWork.baseSystem;
            _appLog = loggerFactory.CreateLogger(nameof(EnumLog.AppLogger));
        }
    }
}