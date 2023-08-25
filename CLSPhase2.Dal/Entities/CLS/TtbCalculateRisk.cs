using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Entities.CLS
{
    public class TtbCalculateRisk
    {
        public object response { get; set; }

        public List<TtbCalculateWorstKyc.EntityRiskLevel> allEntityRisk { get; set; }
    }
}
