namespace CLSPhase2.Dal.Entities
{
    public class CpssModel
    {
        public string CpssUsername { get; set; }
        public string CpssPassword { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
        public Uri CpssInboundUrl { get; set; }
        public Uri CpssOutboundUrl { get; set; }
    }
}
