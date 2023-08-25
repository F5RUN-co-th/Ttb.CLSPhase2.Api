namespace CLSPhase2.Dal.Entities.CLS
{
    public class TtbEntityInfoAusNonAus
    {
        public class KeyValueModel
        {
            public KeyValueModel(string key, string value)
            {
                Key = key;
                Value = value;
            }
            public string Key { get; set; }
            public string Value { get; set; }
        }
        public class Address : TtbEntityInfomation.EntityAddress
        {

        }
        public class TtbEntityInformation
        {
            public long EntityId { get; set; }
            public string TmbIdentificationId { get; set; }
            public string TmbBorrowerFullNameTh { get; set; }
            public string TmbBorrowerFullNameEn { get; set; }
            public string TmbBorrowerNameTh { get; set; }
            public string TmbBorrowerSurnameTh { get; set; }
            public string TmbBusinessCode { get; set; }
            public string TmbCustomerId { get; set; }
            public string EntityType { get; set; }
            public string PartnerType { get; set; }
            public string LongName { get; set; }
            public string TmbBorrowerSurnameEn { get; set; }
            public string TmbIsoCountryIncomeSource { get; set; }
            public string TmbNationality { get; set; }
            public string TmbNationality2 { get; set; }
            public string TmbOccupationCode { get; set; }
            public string TmbCountryOfBirth { get; set; }
            public string behaviorFlag { get; set; } = "N";
            public string relatedPEP { get; set; } = "N";
            public IEnumerable<Address> Address { get; set; }
        }
        public TtbEntityInformation EntityInfo { get; set; }
        public IEnumerable<TtbEntityInformation> BusinessPartnerList { get; set; }
    }
}
