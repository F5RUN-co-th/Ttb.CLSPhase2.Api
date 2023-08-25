namespace CLSPhase2.Dal.Entities.CSGW
{
    public class CSGWCalRiskResponsePersonal
    {
        public CalRiskResponseHeader header { get; set; }

        public CalRiskResponseBody body { get; set; }

        public class CalRiskResponseBody : BaseCalRiskResponse
        {
            public string persSeqKey { get; set; }
            public string statusSWFPers { get; set; }
            public List<MessageSection> messageSection { get; set; }
            public IEnumerable<RelatedCustIndividual> relatedCustIndividual { get; set; }
        }

        public class RelatedCustIndividual : BaseCalRiskResponse
        {
            public string persSeqKey { get; set; }
        }
    }
}
