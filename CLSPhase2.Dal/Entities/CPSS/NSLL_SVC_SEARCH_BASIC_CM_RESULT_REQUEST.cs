using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Entities.CPSS
{
    public class Datum
    {
        public string REQUEST_SYSTEM { get; set; }
        public string REFERENCE_CODE { get; set; }
        public string APP_NUMBER { get; set; }
        public List<int> ENTITY_ID { get; set; }
        public string USER_ID { get; set; }
    }

    public class Input
    {
        public List<Datum> data { get; set; }
    }

    public class NSLL_SVC_SEARCH_BASIC_CM_RESULT_REQUEST
    {
        public object options { get; set; }
        public Input Input { get; set; }
    }

}
