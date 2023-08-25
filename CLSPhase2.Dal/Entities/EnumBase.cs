namespace CLSPhase2.Dal.Entities
{
    public enum EnumBool
    {
        @true,
        @false
    }

    public enum EnumLog
    {
        AppEventLogger,
        AppLogger
    }

    public enum EnumMode
    {
        Debug,
        Release
    }

    public enum EnumSystem
    {
        CLS,
        CSGW,
        CPSS,
        Test
    }

    public enum EnumBasicCmSrchPatterns
    {
        AddressFlag,
        NonAddressFlag
    }

    public enum EnumEventType
    {
        OPERATION,
        BIZ_ERROR,
        BUSINESS
    }

    public enum EnumPayLoadType
    {
        Inbound,
        Outbound
    }

    public enum EnumKYCRiskLevel
    {
        A1 = 1,
        A2 = 2,
        A3 = 3,
        B3 = 4,
        C3 = 6,
    }

    public enum EnumEntityInfoType
    {
        AUS = 1,
        NonAus = 2
    }

    public enum EnumResultStatus
    {
        Success = 200,
        Failed = 500
    }

    public enum EnumCSGWCalRiskResCode
    {
        Success = 0000,
        Fail = 3500
    }
    
    public enum EnumEnvironments
    {
        Local,
        Development,
        SIT,
        UAT
    }
}
