namespace CLSPhase2.Dal.Entities.CLS
{
    public class TtbEntityInfomation
    {

        public class Error
        {
            public string ResourceId { get; set; }
            public bool ShowMessage { get; set; }
        }

        public class ValidationError
        {
            public string Error_ { get; set; }
            public string ModelId_ { get; set; }
            public int EntityId { get; set; }
        }

        public class BusinessPartner
        {
            public int EntityId { get; set; }
            public string LongName { get; set; }
            public string TmbBorrowerFullNameEn { get; set; }
            public string TmbBorrowerFullNameTh { get; set; }
            public string TmbCustomerId { get; set; }
            public string TmbIdentificationId { get; set; }
        }

        public class ClearGlobalConstantCache
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class Data
        {
            public Nullable<bool> TmbIsBusinessPartner { get; set; }
            public string data { get; set; }
        }

        public class DisableCreate
        {
            public bool DocumentDataContext { get; set; }
        }

        public class DocumentDataContext
        {
            public string CreatedBy_ { get; set; }
            public Nullable<DateTime> CreatedDate_ { get; set; }
            public string UpdatedBy_ { get; set; }
            public Nullable<DateTime> UpdatedDate_ { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
            public List<string> Associations_ { get; set; }
        }

        public class EntityAddress
        {
            public int AddressId { get; set; }
            public int EntityId { get; set; }
            public List<string> TmbAddressType { get; set; }
            public string TmbNo { get; set; }
            public string TmbMoo { get; set; }
            public string TmbBuilding { get; set; }
            public string TmbTrokOrSoi { get; set; }
            public string TmbRoad { get; set; }
            public string TmbSubDistrict { get; set; }
            public string TmbDistrict { get; set; }
            public string TmbProvince { get; set; }
            public object TmbSubDistrictNonTh { get; set; }
            public object TmbDistrictNonTh { get; set; }
            public object TmbProvinceNonTh { get; set; }
            public string TmbCountry { get; set; }
            public string TmbPostalCode { get; set; }
            public object TmbPostalCodeNonTh { get; set; }
            public object AddressType { get; set; }
            public object Address1 { get; set; }
            public object Address2 { get; set; }
            public object Country { get; set; }
            public object State { get; set; }
            public object City { get; set; }
            public string Zip { get; set; }
            public object ExternalDataSource { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
            public string EntityType { get; set; }
        }

        public class EntityContactCreateAndEdit
        {
            public int Id { get; set; }
            public int EntityId { get; set; }
            public string TmbTelephoneTypeCode { get; set; }
            public string Name { get; set; }
            public string OfficeNo { get; set; }
            public string TmbExt { get; set; }
            public object Salutation { get; set; }
            public object Email { get; set; }
            public object MobileNo { get; set; }
            public object FaxNo { get; set; }
            public object SystemId { get; set; }
            public object ExternalDataSource { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
        }

        public class EntityDocumentLinkVm
        {
            public int LinkId { get; set; }
            public int EntityId { get; set; }
            public string DocumentId { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
            public string Category { get; set; }
            public string Title { get; set; }
            public string OriginalFileName { get; set; }
            public string TmbDocumentLabel { get; set; }
            public string Description { get; set; }
            public Nullable<double> FileSize { get; set; }
            public int FileVersion { get; set; }
            public string FileId { get; set; }
            public DocumentDataContext DocumentDataContext { get; set; }
        }

        public class IndustryCode
        {
            public string Code { get; set; }
        }

        public class IndustrySelection
        {
            public int IndustryId { get; set; }
            public IndustryCode IndustryCode { get; set; }
            public Nullable<double> Percentage { get; set; }
            public bool IsPrimary { get; set; }
            public int EntityId { get; set; }
            public string Classification { get; set; }
            public object ExternalDataSource { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<string> Inapplicable_ { get; set; }
        }

        public class PayLoad
        {
            public List<ValidationError> _validationErrors { get; set; }

            public DisableCreate disableCreate { get; set; }
            public string PrivateToPublic { get; set; }
            public string ValuationMethodology { get; set; }
            public string Bankruptcy { get; set; }
            public string EnterpriseValueToTotalDebt { get; set; }
            public object IndustrySector { get; set; }
            public object GovernmentBailoutOfFirm { get; set; }
            public string TmbEntityInfoComplete { get; set; }
            public string TmbEnliteRegistrationId { get; set; }
            public int EntityId { get; set; }
            public bool TmbIsBorrower { get; set; }
            public bool TmbIsBusinessPartner { get; set; }
            public string TmbIdentificationId { get; set; }
            public string TmbCustomerId { get; set; }
            public string TmbBorrowerFullNameEn { get; set; }
            public string TmbBorrowerFullNameTh { get; set; }
            public string TmbBorrowerType { get; set; }
            public string EntityType { get; set; }
            public string LongName { get; set; }
            public object ShortName { get; set; }
            public string Segment { get; set; }
            public object LockedDate { get; set; }
            public object SystemId { get; set; }
            public object LockedBy { get; set; }
            public object LegalEntity { get; set; }
            public object TaxId { get; set; }
            public object CustomerSince { get; set; }
            public object CountryOfInc { get; set; }
            public object ProvinceStateOfIncorporation { get; set; }
            public object EstablishmentDate { get; set; }
            public object RegistrationNumber { get; set; }
            public object OnList { get; set; }
            public object Descriptions { get; set; }
            public object Gc23 { get; set; }
            public object Gc22 { get; set; }
            public object ExternalDataSources { get; set; }
            public string TmbBno { get; set; }
            public string BusinessPortfolio { get; set; }
            public Nullable<int> TmbGroupSllStatus { get; set; }
            public string TmbGroupSll1 { get; set; }
            public string TmbGroupSllReason1 { get; set; }
            public object TmbGroupSll2 { get; set; }
            public object TmbGroupSllReason2 { get; set; }
            public object TmbGroupSll3 { get; set; }
            public object TmbGroupSllReason3 { get; set; }
            public string TmbPrefixTh { get; set; }
            public string TmbBorrowerNameTh { get; set; }
            public string TmbEntityType { get; set; }
            public string TmbBorrowerSurnameTh { get; set; }
            public string TmbCustomerType { get; set; }
            public string TmbPrefixEn { get; set; }
            public string TmbIdType { get; set; }
            public string TmbBorrowerSurnameEn { get; set; }
            public Nullable<DateTime> TmbIssueDate { get; set; }
            public bool TmbNoExpiryDate { get; set; }
            public object TmbExpiryDate { get; set; }
            public string CreditPortfolio { get; set; }
            public string TmbBusinessCode { get; set; }
            public string TmbIndustry { get; set; }
            public string TmbIndustryRating { get; set; }
            public string TmbSubIndustry { get; set; }
            public string TmbTargetIndustry { get; set; }
            public string TmbIsicCode { get; set; }
            public string TmbGx4 { get; set; }
            public string TmbCrmBusinessUnit { get; set; }
            public object TmbCountryOfResidence { get; set; }
            public string TmbIsoCountryIncomeSource { get; set; }
            public string TmbNationality { get; set; }
            public object TmbIsoCitizenCode2 { get; set; }
            public string TmbNationality2 { get; set; }
            public object TmbRace { get; set; }
            public string TmbCountryOfBirth { get; set; }
            public object TmbGender { get; set; }
            public Nullable<DateTime> DateOfBirth { get; set; }
            public object TmbEducationLevel { get; set; }
            public string TmbOccupationCode { get; set; }
            public object TmbMaritalStatus { get; set; }
            public object TmbMonthlyIncome { get; set; }
            public object TmbSpouseIdType { get; set; }
            public object TmbSpouseIdentificationId { get; set; }
            public object TmbSpouseNameTh { get; set; }
            public object TmbSpouseDateOfBirth { get; set; }
            public object TmbNumerOfChildren { get; set; }
            public Nullable<DateTime> CreatedDate_ { get; set; }
            public Nullable<DateTime> UpdatedDate_ { get; set; }
            public string UpdatedBy_ { get; set; }
            public string TmbOwnerRm { get; set; }
            public string TmbEmployeeId { get; set; }
            public string TmbReportto { get; set; }
            public object TmbLastApprovedRatingMigratio { get; set; }
            public object TmbLastApprovedRatingDateMigr { get; set; }
            public object Division { get; set; }
            public object PrimaryBankingOfficer { get; set; }
            public object PrimaryCreditOfficer { get; set; }
            public Rules Rules { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
            public List<string> Associations_ { get; set; }
            public object FirmType { get; set; }
            public object CountryOfRisk { get; set; }
            public string Currency { get; set; }
            public object ListingDate { get; set; }
            public object CountryOfListing { get; set; }
            public object StockExchange { get; set; }
            public object StockCode { get; set; }
            public string IndClassification { get; set; }
            public List<EntityContactCreateAndEdit> EntityContactCreateAndEdit { get; set; }
            public List<object> RestrictedEntityUser { get; set; }
            public List<object> EntityOfficers { get; set; }
            public List<TmbEntityEmailInformation> TmbEntityEmailInformation { get; set; }
            public List<EntityAddress> EntityAddress { get; set; }
            public TmbEntityBusinessInformation TmbEntityBusinessInformation { get; set; }
            public List<TmbEntityRevenueStructure> TmbEntityRevenueStructure { get; set; }
            public List<TmbEntityExternalFinancialInfo> TmbEntityExternalFinancialInfo { get; set; }
            public List<object> EntityGroupGrid { get; set; }
            public List<IndustrySelection> IndustrySelection { get; set; }
            public List<object> DirectorAndCompanyOfficer { get; set; }
            public List<object> EntityHierarchy { get; set; }
            public List<object> EntityReOrgHistory { get; set; }
            public TmbEntityCreditMonitoring TmbEntityCreditMonitoring { get; set; }
            public TmbEntityRatingInformation TmbEntityRatingInformation { get; set; }
            public List<object> ExternalRatingUserInput { get; set; }
            public List<TmbEnliteShareholderList> TmbEnliteShareholderList { get; set; }
            public IEnumerable<TmbBusinessPartnerList> TmbBusinessPartnerList { get; set; }
            public List<TmbBusinessPartnerSha> TmbBusinessPartnerSha { get; set; }
            public List<object> TmbBusinessPartnerShj { get; set; }
            public List<TmbBusinessPartnerDir> TmbBusinessPartnerDir { get; set; }
            public List<TmbBusinessPartnerBuy> TmbBusinessPartnerBuy { get; set; }
            public List<TmbBusinessPartnerSup> TmbBusinessPartnerSup { get; set; }
            public TmbEntityBusinessGroup TmbEntityBusinessGroup { get; set; }
            public List<EntityDocumentLinkVm> EntityDocumentLinkVm { get; set; }
        }

        public List<PayLoad> payLoad { get; set; }
        public Status status { get; set; }

        public class Rules
        {
            public TmbValidateDuplicateBno TmbValidateDuplicateBno { get; set; }
            public TmbEmptyBno TmbEmptyBno { get; set; }
            public SetTmbBorrowerFullNameEn SetTmbBorrowerFullNameEn { get; set; }
            public SetTmbBorrowerFullNameTh SetTmbBorrowerFullNameTh { get; set; }
            public ValidateTmbMaximumLevelOfCreditGroupHierarchy ValidateTmbMaximumLevelOfCreditGroupHierarchy { get; set; }
            public SetTmbCreditHierarchyBno SetTmbCreditHierarchyBno { get; set; }
            public SetTmbDefaultBorrowerType SetTmbDefaultBorrowerType { get; set; }
            public SetTmbEntityIndustry SetTmbEntityIndustry { get; set; }
            public SetTmbThaiIdCardExpiryDate SetTmbThaiIdCardExpiryDate { get; set; }
            public TmbSetFullCustomerId TmbSetFullCustomerId { get; set; }
            public ValidateCircularRelationship ValidateCircularRelationship { get; set; }
            public ValidateEntityHierarchyRatios ValidateEntityHierarchyRatios { get; set; }
            public ValidateEntityHierarchyUniqueness ValidateEntityHierarchyUniqueness { get; set; }
            public ValidateTmbBorrowerOrBusinessPartnerStatus ValidateTmbBorrowerOrBusinessPartnerStatus { get; set; }
            public ValidateTmbCitizenIdLastDigit ValidateTmbCitizenIdLastDigit { get; set; }
            public ValidateTmbCitizenSpouseIdLastDigit ValidateTmbCitizenSpouseIdLastDigit { get; set; }
            public ValidateTmbCustomerMailingAddressType ValidateTmbCustomerMailingAddressType { get; set; }
            public ValidateTmbCustomerRegisteredAddressType ValidateTmbCustomerRegisteredAddressType { get; set; }
            public ValidateTmbCustomerTotalPercentOfRevenue ValidateTmbCustomerTotalPercentOfRevenue { get; set; }
            public ValidateTmbMissingEstimatedAmountPerTransaction ValidateTmbMissingEstimatedAmountPerTransaction { get; set; }
            public ValidateTmbMissingSourceOfAsset ValidateTmbMissingSourceOfAsset { get; set; }
            public ValidateTmbMissingEstimatedTransactionPerMonth ValidateTmbMissingEstimatedTransactionPerMonth { get; set; }
            public TmbEntityValidDocLabel TmbEntityValidDocLabel { get; set; }
            public TmbValidateBorrowerMandatoryField TmbValidateBorrowerMandatoryField { get; set; }
            public ValidateTmbChangeOfOwnerRmWithinCreditGroup ValidateTmbChangeOfOwnerRmWithinCreditGroup { get; set; }
            public SetTmbEntityCreditHierarchyInformation SetTmbEntityCreditHierarchyInformation { get; set; }
            public ClearGlobalConstantCache ClearGlobalConstantCache { get; set; }
            public ShowBusinessPartnerBaseInfo ShowBusinessPartnerBaseInfo { get; set; }
            public SetTmbBpInfo SetTmbBpInfo { get; set; }
        }

        public class SetTmbBorrowerFullNameEn
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class SetTmbBorrowerFullNameTh
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class SetTmbBpInfo
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class SetTmbCreditHierarchyBno
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class SetTmbDefaultBorrowerType
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class SetTmbEntityCreditHierarchyInformation
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class SetTmbEntityIndustry
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class SetTmbThaiIdCardExpiryDate
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ShowBusinessPartnerBaseInfo
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class Status
        {
            public List<Error> error { get; set; }
        }

        public class TmbBusinessPartnerBuy
        {
            public int EntityId { get; set; }
            public int TbpEntityId { get; set; }
            public string TbpEntityName { get; set; }
            public Nullable<double> TbpBuyerPerc { get; set; }
            public string TbpBuyerTradeTerms { get; set; }
            public int TbpBuyerCsiChecking { get; set; }
            public string TbpBuyerCsiCode { get; set; }
            public string t_ { get; set; }
            public string OperationType { get; set; }
        }

        public class TmbBusinessPartnerDir
        {
            public int EntityId { get; set; }
            public int TbpEntityId { get; set; }
            public string TbpEntityName { get; set; }
            public Nullable<DateTime> TbpDirectorSince { get; set; }
            public string t_ { get; set; }
            public string OperationType { get; set; }
        }

        public class TmbBusinessPartnerList
        {
            public int Id { get; set; }
            public int EntityId { get; set; }
            public BusinessPartner BusinessPartner { get; set; }
            public string IsShareholder { get; set; }
            public Nullable<double> ShareholderPerc { get; set; }
            public string IsShareholderJtype { get; set; }
            public string IsDirector { get; set; }
            public Nullable<DateTime> DirectorSince { get; set; }
            public string IsBuyer { get; set; }
            public Nullable<double> BuyerPerc { get; set; }
            public string BuyerTradeTerms { get; set; }
            public int? BuyerCsiChecking { get; set; }
            public object BuyerCsiCode { get; set; }
            public string IsSupplier { get; set; }
            public Nullable<double> SuppPerc { get; set; }
            public string SuppTradeTerms { get; set; }
            public int? SuppCsiChecking { get; set; }
            public object SuppCsiCode { get; set; }
            public string IsAut { get; set; }
            public string IsAutPowerAttorney { get; set; }
            public object IsNom { get; set; }
            public object IsPcvPcaPcd { get; set; }
            public object IsMdbCeo { get; set; }
            public object IsUbo { get; set; }
            public object IsAuditor { get; set; }
            public object AuditorCsiChecking { get; set; }
            public object AuditorCsiCode { get; set; }
            public object IsSpsAssetOwner { get; set; }
            public object IsGuarantor { get; set; }
            public Rules Rules { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<string> Inapplicable_ { get; set; }
        }

        public class TmbBusinessPartnerSha
        {
            public int EntityId { get; set; }
            public int TbpEntityId { get; set; }
            public string TbpEntityName { get; set; }
            public Nullable<double> TbpShareholderPerc { get; set; }
            public string t_ { get; set; }
            public string OperationType { get; set; }
        }

        public class TmbBusinessPartnerSup
        {
            public int EntityId { get; set; }
            public int TbpEntityId { get; set; }
            public string TbpEntityName { get; set; }
            public Nullable<double> TbpSuppPerc { get; set; }
            public string TbpSuppTradeTerms { get; set; }
            public int TbpSuppCsiChecking { get; set; }
            public string TbpSuppCsiCode { get; set; }
            public string t_ { get; set; }
            public string OperationType { get; set; }
        }

        public class TmbEmptyBno
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class TmbEnliteShareholderList
        {
            public int Id { get; set; }
            public int EntityId { get; set; }
            public string Title { get; set; }
            public string NameTh { get; set; }
            public string Type { get; set; }
            public Nullable<double> ShareholderPercent { get; set; }
            public string t_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
        }

        public class TmbEntityBusinessGroup
        {
            public int TmbEntityBusinessGroupId { get; set; }
            public object BusinessGroup { get; set; }
            public int EntityId { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
        }

        public class TmbEntityBusinessInformation
        {
            public int TmbEntityBusinessInformationId { get; set; }
            public int EntityId { get; set; }
            public string TmbIndustry { get; set; }
            public string TmbBusinessCode { get; set; }
            public string EntityType { get; set; }
            public Nullable<double> TmbNetFixAsset { get; set; }
            public Nullable<DateTime> TmbNetFixAssetAsOf { get; set; }
            public Nullable<double> TmbNetFixAssetExclLandValue { get; set; }
            public Nullable<double> TmbLandBookValue { get; set; }
            public string TmbBusinessOperatingActivities { get; set; }
            public Nullable<double> TmbSaleVolumePerYear { get; set; }
            public Nullable<double> TmbImportPercent { get; set; }
            public Nullable<double> TmbExportPercent { get; set; }
            public Nullable<double> TmbDomesticIncome { get; set; }
            public Nullable<double> TmbExportIncome { get; set; }
            public string TmbIndirectCountryName { get; set; }
            public object TmbPercentExpIndirectCountry { get; set; }
            public Nullable<DateTime> TmbEstablisedDate { get; set; }
            public string TmbBusinessStatus { get; set; }
            public Nullable<int> TmbTotalEmployee { get; set; }
            public string TmbGroupRevenue { get; set; }
            public string TmbListedStatus { get; set; }
            public object TmbListedSince { get; set; }
            public string TmbRiskCustType { get; set; }
            public string TmbSourceOfIncome { get; set; }
            public object TmbSourceOfIncomeInformation { get; set; }
            public object TmbSourceOfAsset { get; set; }
            public object TmbSourceOfAssetInformation { get; set; }
            public object TmbEstimatedTransactPerMonCr { get; set; }
            public object TmbEstimatedAmtPerTransactCr { get; set; }
            public object TmbEstimatedTransactPerMonDr { get; set; }
            public object TmbEstimatedAmtPerTransactDr { get; set; }
            public string TmbBusinessBrief { get; set; }
            public string TmbManagementExperience { get; set; }
            public object TmbInventoryPolicy { get; set; }
            public string TmbProjectFinanceDetail { get; set; }
            public object TmbComAssDirAndShareholderLt20 { get; set; }
            public string TmbBriefRelationshipTmbHistory { get; set; }
            public Nullable<DateTime> TmbRelationshipWithTmbSince { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
        }

        public class TmbEntityCreditMonitoring
        {
            public int TmbEntityCreditMonitoringId { get; set; }
            public int EntityId { get; set; }
            public object TmbAutomateArFlag { get; set; }
            public object TmbAutomateArDate { get; set; }
            public Nullable<DateTime> TmbPreviousReviewDate { get; set; }
            public Nullable<DateTime> TmbNextReviewDate { get; set; }
            public object TmbExtendedReviewDate { get; set; }
            public string TmbEwsResult { get; set; }
            public Nullable<DateTime> TmbEwsUpdatedDate { get; set; }
            public object TmbEwsReason { get; set; }
            public bool TmbQrsFlag { get; set; }
            public object TmbQrsUpdatedDate { get; set; }
            public object TmbQrsLastResult { get; set; }
            public object TmbQrsNotRequireReason { get; set; }
            public object TmbQrsProcessResultDate { get; set; }
            public object TmbQrsProcessResult { get; set; }
            public string TmbCovenantBreachFlag { get; set; }
            public string TmbCovenantOverdueFlag { get; set; }
            public string TmbBrlResult { get; set; }
            public Nullable<DateTime> TmbBrlUpdatedDate { get; set; }
            public bool TmbKycCddExemption { get; set; }
            public string TmbKycCddRiskLevel { get; set; }
            public Nullable<DateTime> TmbLastKycCddReviewDate { get; set; }
            public Nullable<DateTime> TmbNextKycCddReviewDate { get; set; }
            public Nullable<DateTime> TmbKycLastCheckDate { get; set; }
            public string TmbKycLastCheckStatus { get; set; }
            public Nullable<DateTime> TmbKycDisableTime { get; set; }
            public string TmbKycRequestByUserId { get; set; }
            public object TmbCsiCode { get; set; }
            public object TmbCsiLastCheckDate { get; set; }
            public object TmbCsiLastCheckStatus { get; set; }
            public object TmbCsiRelatedCreditApprovalSys { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
        }

        public class TmbEntityEmailInformation
        {
            public int EntityId { get; set; }
            public int TmbEntityEmailInformationId { get; set; }
            public string TmbEmailType { get; set; }
            public string TmbEmailAddress { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
        }

        public class TmbEntityExternalFinancialInfo
        {
            public int TmbExternalFinancialInfoId { get; set; }
            public int EntityId { get; set; }
            public string TmbFinancialInstitution { get; set; }
            public object TmbFinancialInstitutionRemark { get; set; }
            public string TmbCountry { get; set; }
            public List<string> TmbProductType { get; set; }
            public object TmbProductTypeRemark { get; set; }
            public Nullable<double> TmbLimit { get; set; }
            public Nullable<double> TmbOutstanding { get; set; }
            public List<string> TmbCommittedUncommitted { get; set; }
            public object TmbCollateralValue { get; set; }
            public List<string> TmbCollateralType { get; set; }
            public object TmbCollateralTypeRemark { get; set; }
            public string TmbOtherBankRemark { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
        }

        public class TmbEntityRatingInformation
        {
            public int TmbEntityRatingInformationId { get; set; }
            public int EntityId { get; set; }
            public string TmbRatingStatus { get; set; }
            public string TmbLastApprovedRating { get; set; }
            public Nullable<DateTime> TmbLastApprovedRatingDate { get; set; }
            public string TmbQualitativeClassification { get; set; }
            public Nullable<DateTime> TmbQualitativeClassDate { get; set; }
            public object TmbQualitativeClassReason { get; set; }
            public string TmbQualitativeClassIfrs9 { get; set; }
            public Nullable<DateTime> TmbQualitativeClassDateIfrs9 { get; set; }
            public string TmbQualitativeClassReasonIfrs9 { get; set; }
            public object TmbBotClass { get; set; }
            public object TmbQualAssessmentIfrs9 { get; set; }
            public object TmbQualAssessIfrsDate9 { get; set; }
            public object TmbQualAssessIfrsReason9 { get; set; }
            public object TmbFinalStageIfrs9 { get; set; }
            public object TmbFinalStageIfrsDate9 { get; set; }
            public object TmbFinalStageIfrsReason9 { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<string> Inapplicable_ { get; set; }
        }

        public class TmbEntityRevenueStructure
        {
            public int TmbRevenueStructureId { get; set; }
            public int EntityId { get; set; }
            public string TmbTypeOfProductOrService { get; set; }
            public string TmbProductOrServiceDescription { get; set; }
            public Nullable<double> TmbPercentToRevenue { get; set; }
            public string TmbTradePolicy { get; set; }
            public bool TmbSeasonalPattern { get; set; }
            public object TmbSeasonalPatternDetails { get; set; }
            public string t_ { get; set; }
            public bool IsValid_ { get; set; }
            public bool IsDeleted_ { get; set; }
            public int Access_ { get; set; }
            public int BaseVersionId_ { get; set; }
            public int VersionId_ { get; set; }
            public List<object> Inapplicable_ { get; set; }
        }

        public class TmbEntityValidDocLabel
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class TmbSetFullCustomerId
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class TmbValidateBorrowerMandatoryField
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class TmbValidateDuplicateBno
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateCircularRelationship
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateEntityHierarchyRatios
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateEntityHierarchyUniqueness
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbBorrowerOrBusinessPartnerStatus
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbChangeOfOwnerRmWithinCreditGroup
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbCitizenIdLastDigit
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbCitizenSpouseIdLastDigit
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbCustomerMailingAddressType
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbCustomerRegisteredAddressType
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbCustomerTotalPercentOfRevenue
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbMaximumLevelOfCreditGroupHierarchy
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbMissingEstimatedAmountPerTransaction
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbMissingEstimatedTransactionPerMonth
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }

        public class ValidateTmbMissingSourceOfAsset
        {
            public bool success { get; set; }
            public Data data { get; set; }
            public string ruleName { get; set; }
        }
    }
}
