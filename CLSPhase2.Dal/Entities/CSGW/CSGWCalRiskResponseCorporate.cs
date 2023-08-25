namespace CLSPhase2.Dal.Entities.CSGW
{
    public class CSGWCalRiskResponseCorporate
    {
        public CalRiskResponseHeader header { get; set; }

        public CalRiskResponseBody body { get; set; }

        public class CalRiskResponseBody : BaseCalRiskResponse
        {
            public string corpSeqKey { get; set; }
            public string statusSWFCorp { get; set; }
            public List<MessageSection> messageSection { get; set; }
            public IEnumerable<RelatedCustIndividual> relatedCustIndividual { get; set; }
            public IEnumerable<RelatedCustCorporate> relatedCustCorporate { get; set; }
        }

        public class RelatedCustIndividual : BaseCalRiskResponse
        {
            public string corpSeqKey { get; set; }
        }

        public class RelatedCustCorporate : RelatedCustIndividual
        {

        }
    }
}
