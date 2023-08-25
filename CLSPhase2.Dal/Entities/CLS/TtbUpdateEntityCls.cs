using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Entities.CLS
{
    public class TtbUpdateEntityCls
    {
        public TtbCalculateWorstKyc.EntityRiskLevel entityRiskLevel { get; set; }
        public object result { get; set; }
        public bool SendMailSuccess { get; set; }
        public string EmailTriggerPayload { get; set; }
    }
}