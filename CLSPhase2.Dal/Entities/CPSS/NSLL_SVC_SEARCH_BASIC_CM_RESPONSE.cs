namespace CLSPhase2.Dal.Entities.CPSS
{
    public class Outputs
    {
        public string resultStatus { get; set; }
        public string resultStatusDesc { get; set; }
        public string APP_NUMBER { get; set; }
        public string referenceCode { get; set; }
        public string REQUEST_SYSTEM { get; set; }
        public string TOTAL_ENTITY { get; set; }
        public List<string> ENTITY_LIST { get; set; }
        public List<string> user_fields { get; set; }
    }

    public class NSLL_SVC_SEARCH_BASIC_CM_RESPONSE
    {
        public List<Outputs> Output { get; set; }
    }
}
