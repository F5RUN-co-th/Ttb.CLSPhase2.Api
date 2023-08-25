using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.Entities.CLS;
using CLSPhase2.Dal.Entities.CSGW;
using CLSPhase2.Dal.UnitOfWork;
using CLSPhase2.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace CLSPhase2.Services.Services
{
    public class CSGWService : BaseService, ICSGWService
    {
        private readonly HttpClient _client;
        private const string _corporateUri = "v2.0/internal/kyc/amlo/corporate/cal-risk";
        private const string _personalUri = "v2.0/internal/kyc/amlo/personal/cal-risk";
        public CSGWService(IUnitOfWork unitOfWork,
                           HttpClient httpClient,
                           ILoggerFactory loggerFactory
                            ) : base(unitOfWork, loggerFactory)
        {
            _client = httpClient;
            _client.BaseAddress = _baseSys.csgwModel.CSGWUrl;
        }

        private static List<CSGWCalRiskRequestAddress> ExtractAddress(IEnumerable<TtbEntityInfoAusNonAus.Address> address, string partnerType = "")
        {
            var infoAddrList = new List<CSGWCalRiskRequestAddress>();
            if (partnerType is "AUS" or "")
            {
                foreach (var rows in address)
                {
                    infoAddrList.AddRange(rows.TmbAddressType.Select(addrType => new CSGWCalRiskRequestAddress()
                    {
                        addressPurposeCode = addrType,
                        address = $"{rows.TmbMoo} {rows.TmbBuilding} {rows.TmbTrokOrSoi} {rows.TmbRoad} {rows.TmbSubDistrict} {rows.TmbDistrict}".Trim(),
                        city = rows.TmbProvince ?? "",
                        countryCode = rows.TmbCountry ?? ""
                    }));
                }
            }
            else
            {
                foreach (var rows in address)
                {
                    foreach (var addrType in rows.TmbAddressType)
                    {
                        if (addrType == "REG")
                        {
                            infoAddrList.Add(new CSGWCalRiskRequestAddress()
                            {
                                addressPurposeCode = addrType,
                                address = $"{rows.TmbMoo} {rows.TmbBuilding} {rows.TmbTrokOrSoi} {rows.TmbRoad} {rows.TmbSubDistrict} {rows.TmbDistrict}".Trim(),
                                city = rows.TmbProvince ?? "",
                                countryCode = rows.TmbCountry ?? ""
                            });
                        }
                    }
                }
            }
            return infoAddrList;
        }

        public async Task<TtbRequestCSGW> GenerateRequest(TtbEntityInfoAusNonAus infoAusNonaus)
        {
            string entityType = infoAusNonaus.EntityInfo.EntityType;

            var SequenceCount = new Dictionary<string, int>();

            var businessPartnerList = infoAusNonaus.BusinessPartnerList.Select(s =>
            {
                int count = SequenceCount[s.PartnerType] = SequenceCount.ContainsKey(s.PartnerType) ? SequenceCount[s.PartnerType] + 1 : 1;
                return new { Key = s, SequenceNo = s.PartnerType + count.ToString("d3") };
            }).ToList();

            var custIndividual = businessPartnerList.Where(w => w.Key.EntityType == "P");

            var entityAddress = ExtractAddress(infoAusNonaus.EntityInfo.Address);
            if (entityType is "C")
            {
                var custCorporate = businessPartnerList.Where(w => w.Key.EntityType == "C");

                var corpSeqKey = "";
                if (infoAusNonaus.EntityInfo.TmbCustomerId == "" || infoAusNonaus.EntityInfo.TmbCustomerId is null)
                    corpSeqKey = "corp01";
                else
                    corpSeqKey = infoAusNonaus.EntityInfo.TmbCustomerId;
                var requestBody = new CSGWCalRiskCorporate.CalRiskRequestBody()
                {
                    corpSeqKey = corpSeqKey,
                    corporateName = infoAusNonaus.EntityInfo.TmbBorrowerFullNameTh,
                    corporateNameEng = infoAusNonaus.EntityInfo.TmbBorrowerFullNameEn,
                    corporateId = infoAusNonaus.EntityInfo.TmbIdentificationId,
                    addresses = entityAddress,
                    businessCode1 = infoAusNonaus.EntityInfo.TmbBusinessCode,
                    tellerId = _baseSys.csgwModel.TellerId,
                    relatedCustIndividual = custIndividual.Select(s => new CSGWCalRiskCorporate.RelatedCustIndividual()
                    {
                        corpSeqKey = s.SequenceNo,
                        CustomerFlag = (infoAusNonaus.EntityInfo.TmbCustomerId != "" && infoAusNonaus.EntityInfo.TmbCustomerId is not null) || s.Key.PartnerType == "AUS" ? "Y" : "N",
                        firstName = s.Key.TmbBorrowerNameTh,
                        lastName = s.Key.TmbBorrowerSurnameTh,
                        cardId = s.Key.TmbIdentificationId,
                        type = s.Key.PartnerType,
                        nationalCode = s.Key.TmbNationality,//"TH",
                        addresses = ExtractAddress(s.Key.Address, s.Key.PartnerType),
                        occupationCode = s.Key.TmbOccupationCode,//"401",
                        businessCode1 = s.Key.TmbBusinessCode,
                        businessCode2 = s.Key.TmbBusinessCode,
                        businessCode3 = s.Key.TmbBusinessCode,
                        //===== New
                        firstNameEng = s.Key.LongName,
                        lastNameEng = s.Key.TmbBorrowerSurnameEn,
                        isoCountryIncomeSource = s.Key.TmbIsoCountryIncomeSource,
                        nationalCode2 = s.Key.TmbNationality2,
                        isoCountryOfBirth = s.Key.TmbCountryOfBirth,
                        behaviorFlag = s.Key.behaviorFlag,
                        relatedPEP = s.Key.relatedPEP,
                        //=====
                        EntityId = s.Key.EntityId,
                    }),
                    relatedCustCorporate = custCorporate.Select(s => new CSGWCalRiskCorporate.RelatedCustCorporate()
                    {
                        corpSeqKey = s.SequenceNo,
                        CustomerFlag = (infoAusNonaus.EntityInfo.TmbCustomerId != "" && infoAusNonaus.EntityInfo.TmbCustomerId is not null) || s.Key.PartnerType == "AUS" ? "Y" : "N",
                        corporateName = s.Key.TmbBorrowerFullNameTh,
                        corporateNameEng = s.Key.TmbBorrowerFullNameEn,
                        corporateId = s.Key.TmbIdentificationId,
                        type = s.Key.PartnerType,
                        addresses = ExtractAddress(s.Key.Address, s.Key.PartnerType),
                        businessCode1 = s.Key.TmbBusinessCode,
                        businessCode2 = s.Key.TmbBusinessCode,
                        businessCode3 = s.Key.TmbBusinessCode,
                        //dateOfRegister = "02/11/1992",
                        //isoCountryIncomeSource = "TH",
                        //behaviorFlag = "N",
                        //relatedPEP = "N"
                        EntityId = s.Key.EntityId
                    }),
                    EntityId = infoAusNonaus.EntityInfo.EntityId
                };
                return GenerateRequest(_corporateUri, requestBody);
            }
            else
            {
                var persSeqKey = "";
                if (infoAusNonaus.EntityInfo.TmbCustomerId == "" || infoAusNonaus.EntityInfo.TmbCustomerId is null)
                    persSeqKey = "pers01";
                else
                    persSeqKey = infoAusNonaus.EntityInfo.TmbCustomerId;
                var requestBody = new CSGWCalRiskPersonal.CalRiskRequestBody()
                {
                    persSeqKey = persSeqKey,
                    firstName = infoAusNonaus.EntityInfo.TmbBorrowerNameTh,
                    lastName = infoAusNonaus.EntityInfo.TmbBorrowerSurnameTh,
                    cardId = infoAusNonaus.EntityInfo.TmbIdentificationId,
                    addresses = entityAddress,
                    businessCode1 = infoAusNonaus.EntityInfo.TmbBusinessCode,
                    firstNameEng = infoAusNonaus.EntityInfo.LongName,
                    lastNameEng = infoAusNonaus.EntityInfo.TmbBorrowerSurnameEn,
                    tellerId = _baseSys.csgwModel.TellerId,
                    relatedCustIndividual = custIndividual.Select(s => new CSGWCalRiskPersonal.RelatedCustIndividual()
                    {
                        persSeqKey = s.SequenceNo,
                        CustomerFlag = (infoAusNonaus.EntityInfo.TmbCustomerId == "" || infoAusNonaus.EntityInfo.TmbCustomerId is null) ? "N" : "Y",
                        firstName = s.Key.TmbBorrowerNameTh,
                        lastName = s.Key.TmbBorrowerSurnameTh,
                        cardId = s.Key.TmbIdentificationId,
                        type = s.Key.PartnerType,
                        nationalCode = s.Key.TmbNationality,//"TH",
                        addresses = ExtractAddress(s.Key.Address, s.Key.PartnerType),
                        occupationCode = s.Key.TmbOccupationCode,//"401",
                        businessCode1 = s.Key.TmbBusinessCode,
                        //===== New
                        firstNameEng = s.Key.LongName,
                        lastNameEng = s.Key.TmbBorrowerSurnameEn,
                        isoCountryIncomeSource = s.Key.TmbIsoCountryIncomeSource,
                        nationalCode2 = s.Key.TmbNationality2,
                        isoCountryOfBirth = s.Key.TmbCountryOfBirth,
                        behaviorFlag = s.Key.behaviorFlag,
                        relatedPEP = s.Key.relatedPEP,
                        //=====
                        EntityId = s.Key.EntityId,
                    }),
                    nationalCode = infoAusNonaus.EntityInfo.TmbNationality,
                    occupationCode = infoAusNonaus.EntityInfo.TmbOccupationCode,
                    isoCountryIncomeSource = infoAusNonaus.EntityInfo.TmbIsoCountryIncomeSource,
                    nationalCode2 = infoAusNonaus.EntityInfo.TmbNationality2,
                    isoCountryOfBirth = infoAusNonaus.EntityInfo.TmbCountryOfBirth,
                    behaviorFlag = infoAusNonaus.EntityInfo.behaviorFlag,
                    relatedPEP = infoAusNonaus.EntityInfo.relatedPEP,
                    EntityId = infoAusNonaus.EntityInfo.EntityId
                };
                return GenerateRequest(_personalUri, requestBody);
            }
        }

        public async Task<TtbCalculateRisk> CalculateRisk(TtbRequestCSGW obj)
        {
            if (obj.calRisk is null)
                return null;

            var jsonObjOrigin = JObject.FromObject(obj.calRisk);
            var jsonObjRequest = JObject.FromObject(obj.calRisk);
            var uri = jsonObjRequest["Uri"].ToString();
            jsonObjRequest.Remove("Uri");
            jsonObjRequest.Descendants()
                            .Where(x => x.Type == JTokenType.Property)
                                .Cast<JProperty>()
                                    .Where(x => x.Name == "EntityId").ToList().ForEach(x => x.Remove());
            var resp = await _client.PostAsync(uri, new StringContent(jsonObjRequest.ToString(), Encoding.UTF8, "application/json"));
            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                if (_personalUri == uri)
                {
                    var calriskPersonal = jsonObjOrigin.ToObject<CSGWCalRiskBase<CSGWCalRiskPersonal.CalRiskRequestBody>>();
                    return await Process<CSGWCalRiskResponsePersonal>(content, calriskPersonal);
                }
                else
                {
                    var calriskCorporate = jsonObjOrigin.ToObject<CSGWCalRiskBase<CSGWCalRiskCorporate.CalRiskRequestBody>>();
                    return await Process<CSGWCalRiskResponseCorporate>(content, calriskCorporate);
                }
            }
            else
            {
                return null;
            }
        }

        private TtbRequestCSGW GenerateRequest<T>(string uri, T body)
        {
            return new TtbRequestCSGW()
            {
                calRisk = new CSGWCalRiskBase<T>()
                {
                    Uri = uri,
                    header = new CalRiskRequestHeader()
                    {
                        reqId = DateTime.Now.ToString("yyyyMMddHHmmssfffff", CultureInfo.CreateSpecificCulture("en-US")),
                        appId = _baseSys.AppId,
                        acronym = _baseSys.UserId,
                        selectorFlag = _baseSys.csgwModel.CSGWUseFlag
                    },
                    body = body
                }
            };
        }

        private async Task<TtbCalculateRisk> Process<T>(string content, object calriskModel)
        {
            var response = JsonConvert.DeserializeObject<T>(content);
            var resHeader = response.GetType().GetProperty("header").GetValue(response);
            var IsSucess = EnumCSGWCalRiskResCode.Success == (EnumCSGWCalRiskResCode)Enum.Parse(typeof(EnumCSGWCalRiskResCode), (string)resHeader.GetType().GetProperty("resCode").GetValue(resHeader));
            if (IsSucess)
            {
                var entityRisk = await ProcessRisk(calriskModel, response);
                return new TtbCalculateRisk() { response = response, allEntityRisk = entityRisk };
            }
            else
            {
                var _body = calriskModel.GetType().GetProperty("body").GetValue(calriskModel);
                var Id = (long)_body.GetType().GetProperty("EntityId").GetValue(_body);
                return new TtbCalculateRisk() { response = response, allEntityRisk = new List<TtbCalculateWorstKyc.EntityRiskLevel>() { new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = Id, IsBorrower = true, IsSuccess = false } } };
            }
        }

        private async Task<List<TtbCalculateWorstKyc.EntityRiskLevel>> ProcessRisk<T>(object calriskModel, T csgwResponse)
        {
            var resAllEntityRisk = new List<TtbCalculateWorstKyc.EntityRiskLevel>();

            if (csgwResponse is CSGWCalRiskResponseCorporate respCorp && calriskModel is CSGWCalRiskBase<CSGWCalRiskCorporate.CalRiskRequestBody> reqCorp)
            {
                var borrowerId = reqCorp.body.EntityId;
                if (respCorp.body.corpSeqKey == null || respCorp.body.corpSeqKey == "")
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = borrowerId, IsBorrower = true, IsSuccess = false });
                else
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = borrowerId, maxRisk = respCorp.body.maxRisk, maxRiskRM = respCorp.body.maxRiskRM, IsBorrower = true, IsSuccess = true });

                var mapCustIndiv = from t1 in reqCorp.body.relatedCustIndividual
                                   from t2 in respCorp.body.relatedCustIndividual
                                   where t1.corpSeqKey == t2.corpSeqKey
                                   select new
                                   {
                                       t2.corpSeqKey,
                                       t1.EntityId,
                                       MaxRisk = t2.maxRisk,
                                       MaxRiskRM = t2.maxRiskRM
                                   };
                var mapCustCorp = from t1 in reqCorp.body.relatedCustCorporate
                                  from t2 in respCorp.body.relatedCustCorporate
                                  where t1.corpSeqKey == t2.corpSeqKey
                                  select new
                                  {
                                      t2.corpSeqKey,
                                      t1.EntityId,
                                      MaxRisk = t2.maxRisk,
                                      MaxRiskRM = t2.maxRiskRM
                                  };

                foreach (var cus in mapCustIndiv)
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = cus.EntityId, maxRisk = cus.MaxRisk, maxRiskRM = cus.MaxRiskRM, IsSuccess = true });
                foreach (var corp in mapCustCorp)
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = corp.EntityId, maxRisk = corp.MaxRisk, maxRiskRM = corp.MaxRiskRM, IsSuccess = true });

                var entityNotExitsInCus = reqCorp.body.relatedCustIndividual.Where(a => !mapCustIndiv.Any(p2 => p2.corpSeqKey.Contains(a.corpSeqKey)));
                if (entityNotExitsInCus.Any())
                    foreach (var item in entityNotExitsInCus)
                        resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = item.EntityId, IsSuccess = false });

                var entityNotExitsInCorp = reqCorp.body.relatedCustCorporate.Where(a => !mapCustCorp.Any(p2 => p2.corpSeqKey.Contains(a.corpSeqKey)));
                if (entityNotExitsInCorp.Any())
                    foreach (var item in entityNotExitsInCorp)
                        resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = item.EntityId, IsSuccess = false });
            }
            else if (csgwResponse is CSGWCalRiskResponsePersonal respPers && calriskModel is CSGWCalRiskBase<CSGWCalRiskPersonal.CalRiskRequestBody> reqPers)
            {
                var borrowerId = reqPers.body.EntityId;
                if (respPers.body.persSeqKey == null || respPers.body.persSeqKey == "")
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = borrowerId, IsBorrower = true, IsSuccess = false });
                else
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = borrowerId, maxRisk = respPers.body.maxRisk, maxRiskRM = respPers.body.maxRiskRM, IsBorrower = true, IsSuccess = true });

                var mapCustIndiv = from t1 in reqPers.body.relatedCustIndividual
                                   from t2 in respPers.body.relatedCustIndividual
                                   where t1.persSeqKey == t2.persSeqKey
                                   select new
                                   {
                                       t2.persSeqKey,
                                       t1.EntityId,
                                       MaxRisk = t2.maxRisk,
                                       MaxRiskRM = t2.maxRiskRM
                                   };
                foreach (var cus in mapCustIndiv)
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = cus.EntityId, maxRisk = cus.MaxRisk, maxRiskRM = cus.MaxRiskRM, IsSuccess = true });

                var entityNotExitsInCus = reqPers.body.relatedCustIndividual.Where(a => !mapCustIndiv.Any(p2 => p2.persSeqKey.Contains(a.persSeqKey)));
                if (entityNotExitsInCus.Any())
                    foreach (var item in entityNotExitsInCus)
                        resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = item.EntityId, IsSuccess = false });
            }

            return resAllEntityRisk;
        }
    }
}






















































































































































/*
         public async Task<TtbRequestCSGW> PrepareRequestCSGW3(TtbEntityInfoAusNonAus infoAusNonaus)
        {
            string entityType = infoAusNonaus.EntityInfo.EntityType;
            var SequenceCount = new Dictionary<string, int>();
            var businessPartnerList = infoAusNonaus.BusinessPartnerList.Select(s =>
            {
                int count = SequenceCount[s.PartnerType] = SequenceCount.ContainsKey(s.PartnerType) ? SequenceCount[s.PartnerType] + 1 : 1;
                return new { Key = s, SequenceNo = s.PartnerType + count.ToString("d3") };
            }).ToList();

            var custIndividual = businessPartnerList.Where(w => w.Key.EntityType == "P");

            var entityAddress = ExtractAddress(infoAusNonaus.EntityInfo.Address);
            if (entityType is "C")
            {
                var custCorporate = businessPartnerList.Where(w => w.Key.EntityType == "C");

                var reqCalKycBody = new TtbCalculateWorstKyc.CalculateWorstKycBody<CSGWCalRiskCorporate.CalRiskRequestBody, CSGWCalRiskCorporate.RelatedCustIndividual, CSGWCalRiskCorporate.RelatedCustCorporate>();
                reqCalKycBody.body = new CSGWCalRiskCorporate.CalRiskRequestBody();

                if (infoAusNonaus.EntityInfo.TmbCustomerId == "" || infoAusNonaus.EntityInfo.TmbCustomerId is null)
                    reqCalKycBody.body.corpSeqKey = "corp01";
                else
                    reqCalKycBody.body.corpSeqKey = infoAusNonaus.EntityInfo.TmbCustomerId;
                reqCalKycBody.body.corporateName = infoAusNonaus.EntityInfo.TmbBorrowerFullNameTh;
                reqCalKycBody.body.corporateNameEng = infoAusNonaus.EntityInfo.TmbBorrowerFullNameEn;
                reqCalKycBody.body.corporateId = infoAusNonaus.EntityInfo.TmbIdentificationId;
                reqCalKycBody.body.addresses = entityAddress;
                reqCalKycBody.body.businessCode1 = infoAusNonaus.EntityInfo.TmbBusinessCode;
                var relateCusIndivl = custIndividual.Select(s => new TtbCalculateWorstKyc.CalculateWorstKycIndividual<CSGWCalRiskCorporate.RelatedCustIndividual>()
                {
                    body = new CSGWCalRiskCorporate.RelatedCustIndividual()
                    {
                        corpSeqKey = s.SequenceNo,
                        CustomerFlag = s.Key.PartnerType == "AUS" ? "Y" : "N",
                        firstName = s.Key.TmbBorrowerNameTh,
                        lastName = s.Key.TmbBorrowerSurnameTh,

                        //New
                        firstNameEng = s.Key.LongName,
                        lastNameEng = s.Key.TmbBorrowerSurnameEn,
                        isoCountryIncomeSource = s.Key.TmbIsoCountryIncomeSource,
                        //

                        cardId = s.Key.TmbIdentificationId,
                        type = s.Key.PartnerType,
                        nationalCode = "TH",
                        nationalCode2 = "TH",

                        addresses = ExtractAddress(s.Key.Address, s.Key.PartnerType),

                        occupationCode = "401",
                        businessCode1 = s.Key.TmbBusinessCode,
                        businessCode2 = s.Key.TmbBusinessCode,
                        businessCode3 = s.Key.TmbBusinessCode,
                    },
                    EntityId = s.Key.EntityId,
                });
                reqCalKycBody.relatedCustIndividual = relateCusIndivl;
                var relateCusCorp = custCorporate.Select(s => new TtbCalculateWorstKyc.CalculateWorstKycCustCorporate<CSGWCalRiskCorporate.RelatedCustCorporate>()
                {
                    body = new CSGWCalRiskCorporate.RelatedCustCorporate()
                    {
                        corpSeqKey = s.SequenceNo,
                        CustomerFlag = s.Key.PartnerType == "AUS" ? "Y" : "N",
                        corporateName = s.Key.TmbBorrowerFullNameTh,
                        corporateNameEng = s.Key.TmbBorrowerFullNameEn,
                        corporateId = s.Key.TmbIdentificationId,
                        type = s.Key.PartnerType,
                        addresses = ExtractAddress(s.Key.Address, s.Key.PartnerType),
                        businessCode1 = s.Key.TmbBusinessCode,
                        businessCode2 = s.Key.TmbBusinessCode,
                        businessCode3 = s.Key.TmbBusinessCode,
                        //dateOfRegister = "02/11/1992",
                        //isoCountryIncomeSource = "TH",
                        //behaviorFlag = "N",
                        //relatedPEP = "N"
                    },
                    EntityId = s.Key.EntityId
                });
                reqCalKycBody.relatedCustCorporate = relateCusCorp;
                reqCalKycBody.EntityId = infoAusNonaus.EntityInfo.EntityId;
                var requestBody = new CSGWCalRiskCorporate.CalRiskRequestBody()
                {
                    corpSeqKey = reqCalKycBody.body.corpSeqKey,
                    corporateName = reqCalKycBody.body.corporateName,
                    corporateNameEng = reqCalKycBody.body.corporateNameEng,
                    corporateId = reqCalKycBody.body.corporateId,
                    addresses = reqCalKycBody.body.addresses,
                    businessCode1 = reqCalKycBody.body.businessCode1,
                    tellerId = _tellerId,
                    relatedCustIndividual = reqCalKycBody.relatedCustIndividual.Select(s => new CSGWCalRiskCorporate.RelatedCustIndividual
                    {
                        corpSeqKey = s.body.corpSeqKey,
                        CustomerFlag = s.body.CustomerFlag,
                        firstName = s.body.firstName,
                        lastName = s.body.lastName,
                        cardId = s.body.cardId,
                        type = s.body.type,
                        nationalCode = s.body.nationalCode,
                        nationalCode2 = s.body.nationalCode2,
                        addresses = s.body.addresses,
                        occupationCode = s.body.occupationCode,
                        businessCode1 = s.body.businessCode1,
                        businessCode2 = s.body.businessCode2,
                        businessCode3 = s.body.businessCode3,
                        firstNameEng = s.body.firstNameEng,
                        lastNameEng = s.body.lastNameEng,
                        isoCountryIncomeSource = s.body.isoCountryIncomeSource,
                    }),
                    relatedCustCorporate = reqCalKycBody.relatedCustCorporate.Select(s => new CSGWCalRiskCorporate.RelatedCustCorporate
                    {
                        corpSeqKey = s.body.corpSeqKey,
                        CustomerFlag = s.body.CustomerFlag,
                        corporateName = s.body.corporateName,
                        corporateNameEng = s.body.corporateNameEng,
                        corporateId = s.body.corporateId,
                        type = s.body.type,
                        addresses = s.body.addresses,
                        businessCode1 = s.body.businessCode1,
                        businessCode2 = s.body.businessCode2,
                        businessCode3 = s.body.businessCode3,
                    })
                };
                return GenerateRequest3(_corporateUri, requestBody, reqCalKycBody);
            }
            else
            {
                var reqCalKycBody = new TtbCalculateWorstKyc.CalculateWorstKycBody<CSGWCalRiskPersonal.CalRiskRequestBody, CSGWCalRiskPersonal.RelatedCustIndividual, CSGWCalRiskPersonal.RelatedCustCorporate>();
                reqCalKycBody.body = new CSGWCalRiskPersonal.CalRiskRequestBody();

                if (infoAusNonaus.EntityInfo.TmbCustomerId == "" || infoAusNonaus.EntityInfo.TmbCustomerId is null)
                    reqCalKycBody.body.persSeqKey = "pers01";
                else
                    reqCalKycBody.body.persSeqKey = infoAusNonaus.EntityInfo.TmbCustomerId;
                reqCalKycBody.body.firstName = infoAusNonaus.EntityInfo.TmbBorrowerNameTh;
                reqCalKycBody.body.lastName = infoAusNonaus.EntityInfo.TmbBorrowerSurnameTh;
                reqCalKycBody.body.cardId = infoAusNonaus.EntityInfo.TmbIdentificationId;
                reqCalKycBody.body.addresses = entityAddress;
                reqCalKycBody.body.businessCode1 = infoAusNonaus.EntityInfo.TmbBusinessCode;
                var relateCusIndivl = custIndividual.Select(s => new TtbCalculateWorstKyc.CalculateWorstKycIndividual<CSGWCalRiskPersonal.RelatedCustIndividual>()
                {
                    body = new CSGWCalRiskPersonal.RelatedCustIndividual()
                    {
                        persSeqKey = s.SequenceNo,
                        CustomerFlag = (infoAusNonaus.EntityInfo.TmbCustomerId == "" || infoAusNonaus.EntityInfo.TmbCustomerId is null) ? "N" : "Y",
                        firstName = s.Key.TmbBorrowerNameTh,
                        lastName = s.Key.TmbBorrowerSurnameTh,
                        cardId = s.Key.TmbIdentificationId,
                        type = s.Key.PartnerType,
                        nationalCode = "TH",
                        addresses = ExtractAddress(s.Key.Address, s.Key.PartnerType),
                        occupationCode = "401",//s.Key.TmbOccupationCode,
                        businessCode1 = s.Key.TmbBusinessCode,

                        //New
                        firstNameEng = s.Key.LongName,
                        lastNameEng = s.Key.TmbBorrowerSurnameEn,
                        isoCountryIncomeSource = s.Key.TmbIsoCountryIncomeSource,
                        //
                    },
                    EntityId = s.Key.EntityId,
                });
                reqCalKycBody.relatedCustIndividual = relateCusIndivl;
                reqCalKycBody.EntityId = infoAusNonaus.EntityInfo.EntityId;
                var requestBody = new CSGWCalRiskPersonal.CalRiskRequestBody()
                {
                    persSeqKey = reqCalKycBody.body.persSeqKey,
                    firstName = reqCalKycBody.body.firstName,
                    lastName = reqCalKycBody.body.lastName,
                    cardId = reqCalKycBody.body.cardId,
                    addresses = reqCalKycBody.body.addresses,
                    businessCode1 = reqCalKycBody.body.businessCode1,
                    tellerId = _tellerId,
                    relatedCustIndividual = reqCalKycBody.relatedCustIndividual.Select(s => new CSGWCalRiskPersonal.RelatedCustIndividual
                    {
                        persSeqKey = s.body.persSeqKey,
                        CustomerFlag = s.body.CustomerFlag,
                        firstName = s.body.firstName,
                        lastName = s.body.lastName,
                        cardId = s.body.cardId,
                        type = s.body.type,
                        nationalCode = s.body.nationalCode,
                        addresses = s.body.addresses,
                        occupationCode = s.body.occupationCode,
                        businessCode1 = s.body.businessCode1,
                        firstNameEng = s.body.firstNameEng,
                        lastNameEng = s.body.lastNameEng,
                        isoCountryIncomeSource = s.body.isoCountryIncomeSource,
                    })
                };
                return GenerateRequest3(_personalUri, requestBody, reqCalKycBody);
            }
        }

        private TtbRequestCSGW GenerateRequest3<T1, T2, T3, T4>(string uri, T1 body, TtbCalculateWorstKyc.CalculateWorstKycBody<T2, T3, T4> kycBody)
        {
            var reqHeader = new CalRiskRequestHeader()
            {
                reqId = DateTime.Now.ToString("yyyyMMddHHmmssfffff", CultureInfo.CreateSpecificCulture("en-US")),
                appId = _applicationId,
                acronym = _actionId,
                selectorFlag = _csgwFlag
            };
            return new TtbRequestCSGW()
            {
                calRisk = new CSGWCalRiskBase<T1>()
                {
                    Uri = uri,
                    header = reqHeader,
                    body = body
                },
                modelToCal = new TtbCalculateWorstKyc.CalculateWorstKycRequest<T2, T3, T4>()
                {
                    Uri = uri,
                    header = reqHeader,
                    data = kycBody
                }
            };
        }

        public async Task<TtbCalculateRisk> CalculateRisk3(TtbRequestCSGW obj)
        {
            if (obj.calRisk is null || obj.modelToCal is null)
                return null;

            var jsonContent = JObject.FromObject(obj.calRisk);
            var uri = jsonContent["Uri"].ToString();
            var header = jsonContent["header"].ToString();
            var body = jsonContent["body"].ToString();
            jsonContent.Remove("Uri");
            var resp = await _client.PostAsync(uri, new StringContent(jsonContent.ToString(), Encoding.UTF8, "application/json"));
            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                if (obj.calRisk is CSGWCalRiskBase<CSGWCalRiskCorporate.CalRiskRequestBody>)
                {
                    var objRes = await Process3<CSGWCalRiskResponseCorporate>(content, obj.modelToCal);
                    return new TtbCalculateRisk() { response = objRes.response, allEntityRisk = objRes.allEntityRisk };
                }
                else
                {
                    var objRes = await Process3<CSGWCalRiskResponsePersonal>(content, obj.modelToCal);
                    return new TtbCalculateRisk() { response = objRes.response, allEntityRisk = objRes.allEntityRisk };
                }
            }
            else
            {
                return null;
            }
        }

        private async Task<(T response, List<TtbCalculateWorstKyc.EntityRiskLevel> allEntityRisk)> Process3<T>(string content, object modelToCal)
        {
            var response = JsonConvert.DeserializeObject<T>(content);
            var resHeader = response.GetType().GetProperty("header").GetValue(response);
            var IsSucess = EnumCSGWCalRiskResCode.Success == (EnumCSGWCalRiskResCode)Enum.Parse(typeof(EnumCSGWCalRiskResCode), (string)resHeader.GetType().GetProperty("resCode").GetValue(resHeader));
            if (IsSucess)
            {
                var entityRisk = await ProcessRisk3(modelToCal, response);
                return (response, entityRisk);
            }
            else
            {
                if (modelToCal is TtbCalculateWorstKyc.CalculateWorstKycRequest<CSGWCalRiskCorporate.CalRiskRequestBody, CSGWCalRiskCorporate.RelatedCustIndividual, CSGWCalRiskCorporate.RelatedCustCorporate> reqCorp)
                    return (response, new List<TtbCalculateWorstKyc.EntityRiskLevel>() { new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = reqCorp.data.EntityId, IsBorrower = true, IsSuccess = false } });
                else if (modelToCal is TtbCalculateWorstKyc.CalculateWorstKycRequest<CSGWCalRiskPersonal.CalRiskRequestBody, CSGWCalRiskPersonal.RelatedCustIndividual, CSGWCalRiskPersonal.RelatedCustCorporate> reqPers)
                    return (response, new List<TtbCalculateWorstKyc.EntityRiskLevel>() { new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = reqPers.data.EntityId, IsBorrower = true, IsSuccess = false } });
            }
            return (response, null);
        }

        private async Task<List<TtbCalculateWorstKyc.EntityRiskLevel>> ProcessRisk3<T>(object modelToCal, T csgwResponse)
        {
            var resAllEntityRisk = new List<TtbCalculateWorstKyc.EntityRiskLevel>();
            var jsonContent = JObject.FromObject(modelToCal);

            if (csgwResponse is CSGWCalRiskResponseCorporate respCorp && modelToCal is TtbCalculateWorstKyc.CalculateWorstKycRequest<CSGWCalRiskCorporate.CalRiskRequestBody, CSGWCalRiskCorporate.RelatedCustIndividual, CSGWCalRiskCorporate.RelatedCustCorporate> reqCorp)
            {
                var borrowerId = reqCorp.data.EntityId;
                if (respCorp.body.corpSeqKey == null || respCorp.body.corpSeqKey == "")
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = borrowerId, IsBorrower = true, IsSuccess = false });
                else
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = borrowerId, maxRisk = respCorp.body.maxRisk, maxRiskRM = respCorp.body.maxRiskRM, IsBorrower = true, IsSuccess = true });

                var mapCustIndiv = from t1 in reqCorp.data.relatedCustIndividual
                                   from t2 in respCorp.body.relatedCustIndividual
                                   where t1.body.corpSeqKey == t2.corpSeqKey
                                   select new
                                   {
                                       t2.corpSeqKey,
                                       t1.EntityId,//((TtbCalculateWorstKyc.CalculateWorstKycIndividual)t1).EntityId,
                                       MaxRisk = t2.maxRisk,
                                       MaxRiskRM = t2.maxRiskRM
                                   };
                var mapCustCorp = from t1 in reqCorp.data.relatedCustCorporate
                                  from t2 in respCorp.body.relatedCustCorporate
                                  where t1.body.corpSeqKey == t2.corpSeqKey
                                  select new
                                  {
                                      t2.corpSeqKey,
                                      t1.EntityId,//((TtbCalculateWorstKyc.CalculateWorstKycCustCorporate)t1).EntityId,
                                      MaxRisk = t2.maxRisk,
                                      MaxRiskRM = t2.maxRiskRM
                                  };

                foreach (var cus in mapCustIndiv)
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = cus.EntityId, maxRisk = cus.MaxRisk, maxRiskRM = cus.MaxRiskRM, IsSuccess = true });
                foreach (var corp in mapCustCorp)
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = corp.EntityId, maxRisk = corp.MaxRisk, maxRiskRM = corp.MaxRiskRM, IsSuccess = true });

                var entityNotExitsInCus = reqCorp.data.relatedCustIndividual.Where(a => !mapCustIndiv.Any(p2 => p2.corpSeqKey.Contains(a.body.corpSeqKey)));
                if (entityNotExitsInCus.Any())
                    foreach (var item in entityNotExitsInCus)
                        resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = item.EntityId, IsSuccess = false });

                var entityNotExitsInCorp = reqCorp.data.relatedCustCorporate.Where(a => !mapCustCorp.Any(p2 => p2.corpSeqKey.Contains(a.body.corpSeqKey)));
                if (entityNotExitsInCorp.Any())
                    foreach (var item in entityNotExitsInCorp)
                        resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = item.EntityId, IsSuccess = false });
            }
            else if (csgwResponse is CSGWCalRiskResponsePersonal respPers && modelToCal is TtbCalculateWorstKyc.CalculateWorstKycRequest<CSGWCalRiskPersonal.CalRiskRequestBody, CSGWCalRiskPersonal.RelatedCustIndividual, CSGWCalRiskPersonal.RelatedCustCorporate> reqPers)
            {
                var borrowerId = reqPers.data.EntityId;
                if (respPers.body.persSeqKey == null || respPers.body.persSeqKey == "")
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = borrowerId, IsBorrower = true, IsSuccess = false });
                else
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = borrowerId, maxRisk = respPers.body.maxRisk, maxRiskRM = respPers.body.maxRiskRM, IsBorrower = true, IsSuccess = true });

                var mapCustIndiv = from t1 in reqPers.data.relatedCustIndividual
                                   from t2 in respPers.body.relatedCustIndividual
                                   where t1.body.persSeqKey == t2.persSeqKey
                                   select new
                                   {
                                       t2.persSeqKey,
                                       t1.EntityId,//((TtbCalculateWorstKyc.CalculateWorstKycIndividual)t1).EntityId,
                                       MaxRisk = t2.maxRisk,
                                       MaxRiskRM = t2.maxRiskRM
                                   };
                foreach (var cus in mapCustIndiv)
                    resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = cus.EntityId, maxRisk = cus.MaxRisk, maxRiskRM = cus.MaxRiskRM, IsSuccess = true });

                var entityNotExitsInCus = reqPers.data.relatedCustIndividual.Where(a => !mapCustIndiv.Any(p2 => p2.persSeqKey.Contains(a.body.persSeqKey)));
                if (entityNotExitsInCus.Any())
                    foreach (var item in entityNotExitsInCus)
                        resAllEntityRisk.Add(new TtbCalculateWorstKyc.EntityRiskLevel { EntityId = item.EntityId, IsSuccess = false });
            }

            return resAllEntityRisk;
        }
         */