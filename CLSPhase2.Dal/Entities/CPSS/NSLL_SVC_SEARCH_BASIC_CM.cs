using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSPhase2.Dal.Entities.CPSS
{
    public class NSLL_SVC_SEARCH_BASIC_CM
    {
        public class CustomerList
        {
            public int customerIndex { get; set; }
            public string companyNameEN { get; set; }
            public string companyNameTH { get; set; }
            public string customerType { get; set; }
            public string firstNameEN { get; set; }
            public string firstNameTH { get; set; }
            public string id { get; set; }
            public string idType { get; set; }
            public string lastNameEN { get; set; }
            public string lastNameTH { get; set; }
        }

        public class ENTITYLISTS : ENTITYLIST
        {
            public long Id { get; set; }
        }

        public class ENTITYLIST
        {
            public long ENTITY_ID { get; set; }
            public List<CustomerList> customerList { get; set; }
            public bool searchAddressFlag1 { get; set; }
            public bool searchAddressFlag2 { get; set; }
            public bool searchAddressFlag3 { get; set; }
            public bool searchAddressFlag4 { get; set; }
            public string district1 { get; set; } = "";
            public string district2 { get; set; } = "";
            public string district3 { get; set; } = "";
            public string district4 { get; set; } = "";
            public string houseNo1 { get; set; } = "";
            public string houseNo2 { get; set; } = "";
            public string houseNo3 { get; set; } = "";
            public string houseNo4 { get; set; } = "";
            public string province1 { get; set; } = "";
            public string province2 { get; set; } = "";
            public string province3 { get; set; } = "";
            public string province4 { get; set; } = "";
            public string subdistrict1 { get; set; } = "";
            public string subdistrict2 { get; set; } = "";
            public string subdistrict3 { get; set; } = "";
            public string subdistrict4 { get; set; } = "";
            public string zipcode1 { get; set; } = "";
            public string zipcode2 { get; set; } = "";
            public string zipcode3 { get; set; } = "";
            public string zipcode4 { get; set; } = "";
            public string requestSystem { get; set; } = "SLS_WEB";
            public string searchRLOSStatus { get; set; } = "all";
        }
    }
}
