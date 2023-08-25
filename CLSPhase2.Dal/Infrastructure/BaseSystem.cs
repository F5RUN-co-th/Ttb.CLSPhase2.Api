using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Infrastructure
{
    public class BaseSystem : IBaseSystem
    {
        public string UserId { get; set; }
        public string AppId { get; set; }
        public CpssModel cpssModel { get; set; }
        public CsgwModel csgwModel { get; set; }
    }
}
