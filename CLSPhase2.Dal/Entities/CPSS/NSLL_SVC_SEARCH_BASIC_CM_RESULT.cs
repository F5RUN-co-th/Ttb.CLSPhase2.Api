using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Entities.CPSS
{
    public class NSLL_SVC_SEARCH_BASIC_CM_RESULT
    {
        public List<Output> Output { get; set; }
    }
    public class CustomerList
    {
        public string citizenId { get; set; }
        public string companyNameEN { get; set; }
        public string companyNameTH { get; set; }
        public List<ListCode> listCode { get; set; }
        public List<ListMatch> listMatch { get; set; }
        public string firstNameEN { get; set; }
        public string firstNameTH { get; set; }
        public bool foundCAW { get; set; }
        public bool foundCodeC { get; set; }
        public bool foundCodeF { get; set; }
        public bool foundCodeI { get; set; }
        public bool foundCodeL { get; set; }
        public bool foundCSI { get; set; }
        public bool foundFDR { get; set; }
        public bool foundNoCode { get; set; }
        public bool foundRLOS { get; set; }
        public bool foundSELOS { get; set; }
        public string id { get; set; }
        public string idType { get; set; }
        public string lastNameEN { get; set; }
        public string lastNameTH { get; set; }
        public List<object> listCAWPerson { get; set; }
        public List<ListCSIPerson> listCSIPerson { get; set; }
        public List<object> listFDRPerson { get; set; }
        public List<object> listPersonField { get; set; }
        public List<object> listRLOSPerson { get; set; }
        public List<object> listSELOSPerson { get; set; }
        public int LIST_INDEX { get; set; }
        public List<listCodeC> listCodeC { get; set; }
        public List<object> listCodeF { get; set; }
        public List<object> listCodeI { get; set; }
        public List<object> listCodeL { get; set; }
        public bool matchId { get; set; }
        public bool matchNameEN { get; set; }
        public bool matchNameTH { get; set; }
        public string nameEN { get; set; }
        public string nameTH { get; set; }
        public string passportNo { get; set; }
        public string registrationId { get; set; }
        public string taxId { get; set; }
    }
    public class listCodeC
    {
        public string code { get; set; }
    }
    public class ENTITYLIST
    {
        public string ENTITY_SK { get; set; }
        public int ENTITY_ID { get; set; }
        public string resultStatusCode { get; set; }
        public string resultStatusDesc { get; set; }
        public List<ListExactByCustomer> listExactByCustomer { get; set; }
        public List<ListExactByAddress1> listExactByAddress1 { get; set; }
        public List<ListExactByAddress2> listExactByAddress2 { get; set; }
        public List<ListExactByAddress3> listExactByAddress3 { get; set; }
        public List<ListExactByAddress4> listExactByAddress4 { get; set; }
        public bool exactByAddress1Warning { get; set; }
        public bool exactByAddress2Warning { get; set; }
        public bool exactByAddress3Warning { get; set; }
        public bool exactByAddress4Warning { get; set; }
        public bool searchAddressFlag1 { get; set; }
        public bool searchAddressFlag2 { get; set; }
        public bool searchAddressFlag3 { get; set; }
        public bool searchAddressFlag4 { get; set; }
        public int totalAddressFlag1 { get; set; }
        public int totalAddressFlag2 { get; set; }
        public int totalAddressFlag3 { get; set; }
        public int totalAddressFlag4 { get; set; }
    }

    public class ListCode
    {
        public string CODE_TYPE { get; set; }
        public string CODE_NAME { get; set; }
    }

    public class ListCSIPerson
    {
        public string matchPersonField { get; set; }
        public string MatchScore { get; set; }
        public string matchType { get; set; }
        public string NSLL_PERSON_SK { get; set; }
        public string SOURCE { get; set; }
    }

    public class ListExactByAddress1
    {
        public string resultStatusCode { get; set; }
        public string resultStatusDesc { get; set; }
        public List<object> listCAWPerson { get; set; }
        public List<object> listCSIPerson { get; set; }
        public List<object> listFDRPerson { get; set; }
        public List<object> listPersonField { get; set; }
        public List<object> listRLOSPerson { get; set; }
        public List<object> listSELOSPerson { get; set; }
        public List<listCodeC> listCodeC { get; set; }
        public List<object> listCodeF { get; set; }
        public List<object> listCodeI { get; set; }
        public List<object> listCodeL { get; set; }
        public List<object> listAddress { get; set; }
        public List<object> listCode { get; set; }
        public List<object> listMatch { get; set; }
    }

    public class ListExactByAddress2
    {
        public List<object> listAddress { get; set; }
        public List<object> listCode { get; set; }
        public List<object> listMatch { get; set; }
        public string resultStatusCode { get; set; }
        public string resultStatusDesc { get; set; }
        public List<object> listCAWPerson { get; set; }
        public List<object> listCSIPerson { get; set; }
        public List<object> listFDRPerson { get; set; }
        public List<object> listPersonField { get; set; }
        public List<object> listRLOSPerson { get; set; }
        public List<object> listSELOSPerson { get; set; }
        public List<listCodeC> listCodeC { get; set; }
        public List<object> listCodeF { get; set; }
        public List<object> listCodeL { get; set; }
        public List<object> listCodeI { get; set; }
    }

    public class ListExactByAddress3
    {
        public List<object> listAddress { get; set; }
        public List<object> listCode { get; set; }
        public string resultStatusCode { get; set; }
        public string resultStatusDesc { get; set; }
        public List<object> listMatch { get; set; }
        public List<object> listCAWPerson { get; set; }
        public List<object> listCSIPerson { get; set; }
        public List<object> listFDRPerson { get; set; }
        public List<object> listPersonField { get; set; }
        public List<object> listRLOSPerson { get; set; }
        public List<object> listSELOSPerson { get; set; }
        public List<listCodeC> listCodeC { get; set; }
        public List<object> listCodeF { get; set; }
        public List<object> listCodeL { get; set; }
        public List<object> listCodeI { get; set; }
    }

    public class ListExactByAddress4
    {
        public List<object> listAddress { get; set; }
        public List<object> listCode { get; set; }
        public string resultStatusCode { get; set; }
        public string resultStatusDesc { get; set; }
        public List<object> listMatch { get; set; }
        public List<object> listCAWPerson { get; set; }
        public List<object> listCSIPerson { get; set; }
        public List<object> listFDRPerson { get; set; }
        public List<object> listPersonField { get; set; }
        public List<object> listRLOSPerson { get; set; }
        public List<object> listSELOSPerson { get; set; }
        public List<listCodeC> listCodeC { get; set; }
        public List<object> listCodeF { get; set; }
        public List<object> listCodeL { get; set; }
        public List<object> listCodeI { get; set; }
    }

    public class ListExactByCustomer
    {
        public int customerIndex { get; set; }
        public string customerNameTH { get; set; }
        public bool exactByCustomerWarning { get; set; }
        public string id { get; set; }
        public string idType { get; set; }
        public string lastNameEN { get; set; }
        public string lastNameTH { get; set; }
        public string customerType { get; set; }
        public string resultStatusCode { get; set; }
        public string resultStatusDesc { get; set; }
        public List<CustomerList> customerList { get; set; }
        public string customerNameEN { get; set; }
    }

    public class ListMatch
    {
        public string NSLL_PERSON_SK { get; set; }
        public string matchType { get; set; }
        public string MATCH_SYSTEM { get; set; }
    }

    public class Output
    {
        public string APP_NUMBER { get; set; }
        public string REQUEST_SYSTEM { get; set; }
        public string REFERENCE_CODE { get; set; }
        public string resultStatusDesc { get; set; }
        public string resultStatusCode { get; set; }
        public int TOTAL_ENTITY { get; set; }
        public List<ENTITYLIST> ENTITY_LIST { get; set; }
        public List<string> user_fields { get; set; }
    }
}
