using CLSPhase2.Dal.Entities.CPSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Entities.CLS
{
    public class TtbUpdateEntityCsi
    {
        public TtbUpdateEntityScheme updateScheme { get; set; }
        public object result { get; set; }

        public bool IsSuccess { get; set; }
    }
}
