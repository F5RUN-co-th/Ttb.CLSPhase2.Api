using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Entities.CPSS
{
    public class TtbMapEntityIndex
    {
        public int Id { get; set; }
        public long EntityId { get; set; }
        public ENTITYLIST? Result { get; set; }
        public string ReferenceCode { get; set; }
    }
}
