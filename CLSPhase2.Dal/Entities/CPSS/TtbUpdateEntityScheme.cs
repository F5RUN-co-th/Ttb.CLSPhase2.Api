using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Entities.CPSS
{
    public class TtbUpdateEntityScheme
    {
        public int Id { get; set; }
        public long EntityId { get; set; }
        public Dictionary<string, object> UpdateScheme { get; set; }
    }
}
