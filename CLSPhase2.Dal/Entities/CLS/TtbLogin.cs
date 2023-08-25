namespace CLSPhase2.Dal.Entities.CLS
{
    public class TtbUserLogin
    {
        public PayLoad payLoad { get; set; }
        public Status status { get; set; }
        public class PayLoad
        {
            public User user { get; set; }
            public string token { get; set; }
            public bool devMode { get; set; }
        }
        public class Status
        {
            public List<Warn> warn { get; set; }
        }
        public class Warn
        {
            public string ResourceId { get; set; }
            public bool ShowMessage { get; set; }
        }
        public class User
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string TenantId { get; set; }
            public bool IsSuperAdministrator { get; set; }
            public int ConfigID { get; set; }
            public List<UserAuthorization> UserAuthorization { get; set; }
        }
        public class UserAuthorization
        {
            public int Id { get; set; }
            public List<string> BusinessPortfolio { get; set; }
            public string CreditPortfolio { get; set; }
            public string Role { get; set; }
            public string UserId { get; set; }
            public List<object> Inapplicable_ { get; set; }
            public string t_ { get; set; }
        }
    }
}
