namespace CLSPhase2.Api.Models
{
    public class ApiSettings
    {
        public string Key { get; set; }
        public string Vector { get; set; }

        public Database Database { get; set; }
        public CreditLenSettings CreditLenSettings { get; set; }

        public CSGWSettings CSGWSettings { get; set; }
    }
    public class Database
    {
        public string PostgresConnectionStr { get; set; }
        public string SchemaCpss { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
    }
    public class CreditLenSettings
    {
        public string Url { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }
    public class CSGWSettings
    {
        public string Url { get; set; }
    }
}
