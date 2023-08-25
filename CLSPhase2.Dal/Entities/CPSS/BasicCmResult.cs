using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Entities.CPSS
{
    public class BasicCmResult
    {
        public BasicCmResult()
        {
            data = new List<Data>();
        }

        public List<Data> data { get; set; }

        public class Data
        {
            public int Id { get; set; }
            public string referenceCode { get; set; }
            public string outbound { get; set; }
            //public NSLL_SVC_SEARCH_BASIC_CM_REQUEST<NSLL_SVC_SEARCH_BASIC_CM.ENTITYLISTS> outbound { get; set; }
            public NSLL_SVC_SEARCH_BASIC_CM_RESULT? inbound { get; set; }
            public bool IsSuccess { get; set; }
        }
    }
}
