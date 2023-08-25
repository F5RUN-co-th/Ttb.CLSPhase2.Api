namespace CLSPhase2.Dal.Entities.CSGW
{
    public class CSGWCalRiskBase<T>
    {
        public string Uri { get; set; }

        public CalRiskRequestHeader header { get; set; }

        public T body { get; set; }
    }
    public class CalRiskRequestHeader
    {
        public string appId { get; set; } //{ get; } = "A0391";
        public string reqId { get; set; }
        public string productCode { get; } = "CLS";
        public string acronym { get; set; }
        public string selectorFlag { get; set; } //{ get; } = "AAII";// "AIII";
        public string alertFlag { get; } = "Y";
    }

    public class CSGWCalRiskRequestAddress
    {
        public string addressPurposeCode { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string countryCode { get; set; }
    }
}

