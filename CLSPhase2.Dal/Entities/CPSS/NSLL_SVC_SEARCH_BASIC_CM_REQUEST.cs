using System.Text.Json.Serialization;

namespace CLSPhase2.Dal.Entities.CPSS
{
    public class NSLL_SVC_SEARCH_BASIC_CM_REQUEST<T>
    {
        
        public NSLL_SVC_SEARCH_BASIC_CM_REQUEST(string applicationId, string actionId, int totalEntity, IEnumerable<T> request)
        {
            options = new Options();
            Input = new NSLL_SVC_SEARCH_BASIC_CM_REQUEST<T>.input<T>
            {
                data = new List<NSLL_SVC_SEARCH_BASIC_CM_REQUEST<T>.Data<T>>
                {
                    new NSLL_SVC_SEARCH_BASIC_CM_REQUEST<T>.Data<T>()
                    {
                        APP_NUMBER = applicationId,
                        USER_ID = actionId,
                        TOTAL_ENTITY = totalEntity,
                        ENTITY_LIST = request
                    }
                }
            };
        }

        public Options options { get; set; }

        public input<T> Input { get; set; }

        public class input<T1>
        {
            public List<Data<T>> data { get; set; }
        }

        public class Data<T2>
        {
            public string REQUEST_SYSTEM { get; set; } = "SLS_WEB";

            //public string referenceCode { get; set; }
            public string APP_NUMBER { get; set; }
            public int TOTAL_ENTITY { get; set; }
            public string USER_ID { get; set; }
            public IEnumerable<T2> ENTITY_LIST { get; set; }
        }

        public class Options
        {
            public Options()
            {

            }
        }
    }
}
