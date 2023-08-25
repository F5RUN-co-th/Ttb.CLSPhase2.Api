using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Infrastructure
{
    public interface IConnectionFactory
    {
        public IDbConnection GetConnection { get; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
    }
}
