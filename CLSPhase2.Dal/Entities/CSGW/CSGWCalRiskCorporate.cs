namespace CLSPhase2.Dal.Entities.CSGW
{
    public class CSGWCalRiskCorporate
    {
        public class CalRiskRequestBody
        {
            public string corpSeqKey { get; set; }
            public string corporateName { get; set; }
            public string corporateNameEng { get; set; }
            public string corporateId { get; set; }
            public IEnumerable<CSGWCalRiskRequestAddress> addresses { get; set; }
            public string businessCode1 { get; set; }
            public string businessCode2 { get; set; }
            public string businessCode3 { get; set; }
            public string tellerId { get; set; } = "";
            public IEnumerable<RelatedCustIndividual> relatedCustIndividual { get; set; }
            public IEnumerable<RelatedCustCorporate> relatedCustCorporate { get; set; }
            public long EntityId { get; set; }
        }

        public class RelatedCustCorporate
        {
            public string corpSeqKey { get; set; }
            public string CustomerFlag { get; set; }
            public string corporateName { get; set; }
            public string corporateNameEng { get; set; }
            public string corporateId { get; set; }
            public string type { get; set; }
            public IEnumerable<CSGWCalRiskRequestAddress> addresses { get; set; }
            public string businessCode1 { get; set; }
            public string businessCode2 { get; set; }
            public string businessCode3 { get; set; }
            public string dateOfRegister { get; set; }
            public string isoCountryIncomeSource { get; set; }
            public string behaviorFlag { get; set; }
            public string relatedPEP { get; set; }
            public long EntityId { get; set; }
        }

        public class RelatedCustIndividual
        {
            public string corpSeqKey { get; set; }
            public string CustomerFlag { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string firstNameEng { get; set; }
            public string lastNameEng { get; set; }
            public string cardId { get; set; }
            public string type { get; set; }
            public string nationalCode { get; set; }
            public string nationalCode2 { get; set; }
            public IEnumerable<CSGWCalRiskRequestAddress> addresses { get; set; }
            public string occupationCode { get; set; }
            public string businessCode1 { get; set; }
            public string businessCode2 { get; set; }
            public string businessCode3 { get; set; }
            public string isoCountryIncomeSource { get; set; }
            public string behaviorFlag { get; set; }
            public string relatedPEP { get; set; }
            public string dateOfBirth { get; set; }
            public string isoCountryOfBirth { get; set; }
            public long EntityId { get; set; }
        }
    }
}
