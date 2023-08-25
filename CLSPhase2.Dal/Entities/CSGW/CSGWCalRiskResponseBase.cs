namespace CLSPhase2.Dal.Entities.CSGW
{
    public class BaseCalRiskResponse : BaseCalRiskResponseCode
    {
        public int maxRisk { get; set; }
        public string maxRiskRM { get; set; }
        public string swfRiskResult { get; set; }
        public string statusSWF { get; set; }
        public string sourceOfRisk { get; set; }

    }

    public class BaseCalRiskResponseCode
    {
        public string resCode1 { get; set; }
        public string resDesc1 { get; set; }
        public string resCode2 { get; set; }
        public string resDesc2 { get; set; }
    }

    public class CalRiskResponseHeader : CalRiskRequestHeader
    {
        public string resCode { get; set; }
        public string resDesc { get; set; }
    }

    public class MessageSection
    {
        public string msgDesc { get; set; }
    }
}