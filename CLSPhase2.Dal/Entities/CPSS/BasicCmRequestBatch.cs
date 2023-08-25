namespace CLSPhase2.Dal.Entities.CPSS
{
    public class BasicCmRequestBatch
    {
        public BasicCmRequestBatch()
        {
            data = new List<Data>();
        }

        public List<Data> data { get; set; }

        public class Data
        {
            public string outbound { get; set; }
            public string request { get; set; }
            public bool IsSuccess { get; set; }
            public NSLL_SVC_SEARCH_BASIC_CM_RESPONSE? response { get; set; }
        }
    }
}
