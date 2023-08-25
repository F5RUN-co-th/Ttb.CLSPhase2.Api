using ApiModelUpdateLib;
using AutoMapper;
using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.Entities.CLS;
using CLSPhase2.Dal.Entities.CPSS;
using CLSPhase2.Dal.UnitOfWork;
using CLSPhase2.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Moodys.CL.RestApi;
using Moodys.CL.RestApi.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using static CLSPhase2.Dal.Entities.CLS.TtbEntityInfomation;
using static CLSPhase2.Dal.Entities.CLS.TtbUserLogin;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CLSPhase2.Services.Services
{
    public class CreditLensService : BaseService, ICreditLensService
    {
        private string _userAuthToken;
        private readonly string _userId;
        private readonly string _password;

        private const string _logoutUri = "api/security/logout";
        private const string _authenticateUri = "api/security/authenticate";
        private const string _getUpdateEntityUri = "api/data/vm/{0}/{1}";
        private const string _getRefDataUri = "api/refData/lookupAsList/{0}";
        private const string _domainExecuteRuleUri = "api/domain/rules/execute/EntityEdit";

        private const string blank = "";
        private object[] rule = { "TmbKycCsgwEmailTrigger" };
        private const string ViewModel = "EntityEdit";
        private const string EntityAddress = "EntityAddress";
        private const string TmbBusinessPartnerList = "TmbBusinessPartnerList";
        private readonly string[] filterAddressNonAus = new string[] { "REG" };
        private readonly string[] filterAddressAus = new string[] { "REG", "PRI", "OFF" };//{ "PRI", "REG", "FAC" };
        private readonly IEnumerable<string> attributes = new[] { "TmbIdentificationId", "TmbBorrowerFullNameTh", "TmbBorrowerFullNameEn", "TmbBusinessCode", "EntityType", "TmbCustomerId", "TmbBorrowerNameTh", "TmbBorrowerSurnameTh" };

        private readonly HttpClient _client;
        private readonly IMapper _mapper;
        public CreditLensService(IConfiguration config,
                                 IUnitOfWork unitOfWork,
                                 IMapper mapper,
                                 HttpClient httpClient,
                                 ILoggerFactory loggerFactory
                                 ) : base(unitOfWork, loggerFactory)
        {
            _userId = config["ApiSettings:CreditLenSettings:UserId"] ?? "";
            _password = config["ApiSettings:CreditLenSettings:Password"] ?? "";
            _client = httpClient;
            _mapper = mapper;
        }

        public async Task SetRequestHeader()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userAuthToken);
        }

        public async Task<TtbUserLogin> Login()
        {
            var resp = await _client.PostAsJsonAsync(_authenticateUri, new CreditLensRequest
            {
                { "UserName", _userId },
                { "Password", _password }
            });
            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TtbUserLogin>(content);
                if (response.status != null)
                {
                    var _result = response.status.warn.First().ResourceId;
                    return null;
                }
                else
                {
                    _userAuthToken = response.payLoad.token;
                    return response;
                }
            }
            else return null;
        }

        public async Task<IEnumerable<TtbEntityInfomation.TmbBusinessPartnerList>> RetrieveBusinessPartnerList(long entityId)
        {
            var resp = await _client.GetAsync(String.Format(_getUpdateEntityUri, ViewModel, entityId));
            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TtbEntityInfomation>(content);
                if (response is null)
                    return null;
                else
                    return response.payLoad.SelectMany(s => s.TmbBusinessPartnerList);
            }
            else return null;
        }

        public async Task<(IEnumerable<TtbBusinessPartnerFilterList> entityInfo, IEnumerable<TtbBusinessPartnerFilterList> partnerList)> FilterBusinessPartner(long entityId, IEnumerable<TtbEntityInfomation.TmbBusinessPartnerList> businessPartnerList, EnumSystem logicSys)
        {
            var entityList = new List<TtbBusinessPartnerFilterList>
            {
                new TtbBusinessPartnerFilterList { EntityId = entityId }
            };
            foreach (var partner in businessPartnerList)
            {
                var Id = partner.BusinessPartner.EntityId;
                //AUS
                if (partner.IsAutPowerAttorney is "Y")
                {
                    entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "AUS" });
                }
                //AUT
                else if (partner.IsAut is "Y")
                {
                    entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "AUT" });
                }
                //SHA
                else if (partner.IsShareholder is "Y" && partner.ShareholderPerc >= 0.2)
                {
                    entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "SHA" });
                }
                //SHJ
                else if (partner.IsShareholderJtype is "Y" && partner.ShareholderPerc >= 0.5)
                {
                    entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "SHJ" });
                }
                //DIR
                else if (partner.IsDirector is "Y")
                {
                    entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "DIR" });
                }
                //UBO
                else if (partner.IsUbo is "Y")
                {
                    entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "UBO" });
                }
                //GUA
                else if (partner.IsGuarantor is "Y")
                {
                    entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "GUA" });
                }
                //COL
                else if (partner.IsSpsAssetOwner is "Y")
                {
                    entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "COL" });
                }


                if (logicSys is EnumSystem.CPSS)
                {
                    //AUD
                    if (partner.IsAuditor is "Y")
                        entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "AUD" });
                    //BUY
                    else if (partner.IsBuyer is "Y")
                        entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "BUY" });
                    //SUPP
                    else if (partner.IsSupplier is "Y")
                        entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "SUPP" });
                }
                else
                {
                    //CEO
                    if (partner.IsMdbCeo is "Y")
                        entityList.Add(new TtbBusinessPartnerFilterList { EntityId = Id, PartnerType = "CEO" });
                }
            }
            return (entityList.Where(w => w.PartnerType is null), entityList.Where(w => w.PartnerType is not null));
        }

        public async Task<List<TtbEntityInfoAusNonAus.TtbEntityInformation>> RetrieveEntityInformation(IEnumerable<TtbBusinessPartnerFilterList> entityInfo, EnumSystem logicSys)
        {
            foreach (var item in entityInfo)
            {
                return new List<TtbEntityInfoAusNonAus.TtbEntityInformation>() { await GetInformation(item, logicSys) };
            }
            return null;
        }

        public async Task<(IEnumerable<TtbEntityInfoAusNonAus.TtbEntityInformation> entityList, TtbEntityInfoAusNonAus infoAusNonaus)> RetrieveEntityBusinessPartnerInformation(List<TtbEntityInfoAusNonAus.TtbEntityInformation> entityInfoList, IEnumerable<TtbBusinessPartnerFilterList> partnerList, EnumSystem logicSys)
        {
            var partnersList = new List<TtbEntityInfoAusNonAus.TtbEntityInformation>();
            foreach (var item in partnerList)
            {
                var obj = await GetInformation(item, logicSys);
                obj.PartnerType = item.PartnerType;
                partnersList.Add(obj);
                entityInfoList.Add(obj);
            }
            return (entityInfoList, new TtbEntityInfoAusNonAus
            {
                EntityInfo = entityInfoList.First(),
                BusinessPartnerList = partnersList
            });
        }

        private async Task<TtbEntityInfoAusNonAus.TtbEntityInformation> GetInformation(TtbBusinessPartnerFilterList item, EnumSystem logicSys)
        {
            var resp = await _client.GetAsync(String.Format(_getUpdateEntityUri, ViewModel, item.EntityId));
            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TtbEntityInfomation>(content);
                var info = response.payLoad.FirstOrDefault();
                if (info.EntityId > 0)
                {
                    var addr = response.payLoad.SelectMany(m => _mapper.Map<IEnumerable<TtbEntityInfoAusNonAus.Address>>(m.EntityAddress));
                    var newAddressList = ExtractAndGroupByAddress(addr, item);
                    if (logicSys is EnumSystem.CPSS)
                    {
                        newAddressList = newAddressList.Select(async s =>
                        {
                            s.TmbNo = $"{s.TmbNo} {s.TmbMoo} {s.TmbBuilding} {s.TmbTrokOrSoi} {s.TmbRoad}";
                            s.TmbDistrict = await ReferenceDataExtraction(nameof(s.TmbDistrict), s.TmbDistrict);
                            s.TmbSubDistrict = await ReferenceDataExtraction(nameof(s.TmbSubDistrict), s.TmbSubDistrict);
                            s.TmbProvince = await ReferenceDataExtraction(nameof(s.TmbProvince), s.TmbProvince);
                            s.TmbPostalCode = await ReferenceDataExtraction(nameof(s.TmbPostalCode), s.TmbPostalCode);
                            return s;
                        }).Select(t => t.Result).ToList();
                    }

                    return new TtbEntityInfoAusNonAus.TtbEntityInformation()
                    {
                        EntityId = info.EntityId,
                        TmbIdentificationId = info.TmbIdentificationId,
                        TmbBorrowerFullNameTh = info.TmbBorrowerFullNameTh,
                        TmbBorrowerFullNameEn = info.TmbBorrowerFullNameEn,
                        TmbBusinessCode = info.TmbBusinessCode,
                        EntityType = info.EntityType,
                        TmbCustomerId = info.TmbCustomerId,
                        TmbBorrowerNameTh = info.TmbBorrowerNameTh,
                        TmbBorrowerSurnameTh = info.TmbBorrowerSurnameTh,
                        Address = newAddressList,
                        //New
                        LongName = info.LongName,
                        TmbBorrowerSurnameEn = info.TmbBorrowerSurnameEn,
                        TmbIsoCountryIncomeSource = info.TmbIsoCountryIncomeSource,
                        TmbNationality = info.TmbNationality,
                        TmbNationality2 = info.TmbNationality2,
                        TmbOccupationCode = info.TmbOccupationCode,
                        TmbCountryOfBirth = info.TmbCountryOfBirth,
                    };
                }
                else
                    return null;
            }
            else return null;
        }

        private async Task<string> ReferenceDataExtraction(string referenceGroup, string key)
        {
            var resp = await _client.PostAsJsonAsync(String.Format(_getRefDataUri, referenceGroup), new CreditLensRequest
                        {
                            { "PayLoad", new Dictionary<string, object>()
                                    {
                                        { "Key",  key}
                                    }
                            }
                        });
            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadAsStringAsync();
                var referenceData = JsonConvert.DeserializeObject<List<TtbEntityInfoAusNonAus.KeyValueModel>>(content);
                var value = referenceData.FirstOrDefault()?.Value;
                return value ?? "";
            }
            return "";
        }

        private List<TtbEntityInfoAusNonAus.Address> ExtractAndGroupByAddress(IEnumerable<TtbEntityInfoAusNonAus.Address> addr, TtbBusinessPartnerFilterList item)
        {
            IEnumerable<TtbEntityInfoAusNonAus.Address> addrFiltered;
            if (item.PartnerType is "AUS" or "" or null)
                addrFiltered = addr.Where(w => filterAddressAus.Any(a => w.TmbAddressType.Contains(a)));
            else
                addrFiltered = addr.Where(w => filterAddressNonAus.Any(a => w.TmbAddressType.Contains(a)));

            var grpAddr = addrFiltered.GroupBy(g => new
            {
                TmbAddressType = String.Join(",", g.TmbAddressType.ToArray()),
                g.TmbMoo,
                g.TmbBuilding,
                g.TmbTrokOrSoi,
                g.TmbRoad,
                g.TmbSubDistrict,
                g.TmbDistrict,
                g.TmbProvince,
                g.TmbCountry,
                g.Zip
            },
            g => g, (key, T) => new { key, _d = T.FirstOrDefault(), obj = T })
            .Select(s => new TtbEntityInfoAusNonAus.Address
            {
                TmbAddressType = s.key.TmbAddressType.Split(",").ToList(),
                TmbMoo = s.key.TmbMoo,
                TmbBuilding = s.key.TmbBuilding,
                TmbTrokOrSoi = s.key.TmbTrokOrSoi,
                TmbRoad = s.key.TmbRoad,
                TmbSubDistrict = s.key.TmbSubDistrict,
                TmbDistrict = s.key.TmbDistrict,
                TmbProvince = s.key.TmbProvince,
                TmbCountry = s.key.TmbCountry,
                Zip = s.key.Zip
            }).ToList();

            IEnumerable<string> newAddrFiltered;
            if (item.PartnerType is "AUS" or "" or null)
                newAddrFiltered = filterAddressAus.Where(a => !grpAddr.Any(p2 => p2.TmbAddressType.Contains(a)));
            else
                newAddrFiltered = filterAddressNonAus.Where(a => !grpAddr.Any(p2 => p2.TmbAddressType.Contains(a)));

            if (newAddrFiltered.Any())
            {
                foreach (var newAddr in newAddrFiltered)
                {
                    grpAddr.Add(new TtbEntityInfoAusNonAus.Address
                    {
                        TmbAddressType = new List<string>() { newAddr }
                    });
                }
            }
            return grpAddr;
        }

        public async Task<List<TtbCalculateWorstKyc.EntityRiskLevel>> CalculateWorstKyc(List<TtbCalculateWorstKyc.EntityRiskLevel> allEntityRisk)
        {
            var maxRisk = allEntityRisk.OrderByDescending(o => o.kycLevel).FirstOrDefault();
            if (maxRisk != null)
                allEntityRisk = allEntityRisk.Select(s =>
                {
                    if (s.IsBorrower is true)
                        s.maxRiskRM = maxRisk.maxRiskRM;
                    return s;
                }).ToList();

            return allEntityRisk;
        }

        public async Task<List<TtbUpdateEntityCls>> UpdateEntityKYCRisk(List<TtbCalculateWorstKyc.EntityRiskLevel> worstKycResult, EnumMode mode)
        {
            var updateResultList = new List<TtbUpdateEntityCls>();
            foreach (var item in worstKycResult)
            {
                Dictionary<string, object> updateScheme = new();
                if (item.IsSuccess is false)
                {
                    updateScheme = new()
                        {
                            { nameof(TtbEntityInfomation.TmbEntityCreditMonitoring), new Dictionary<string, object>()
                            {
                                { nameof(TtbEntityInfomation.TmbEntityCreditMonitoring.TmbKycLastCheckStatus), (int)EnumResultStatus.Failed },
                                { "OperationType", "Update" }
                            }}
                        };
                }
                else
                {
                    updateScheme = new()
                        {
                            { nameof(TtbEntityInfomation.TmbEntityCreditMonitoring), new Dictionary<string, object>()
                            {
                                { nameof(TtbEntityInfomation.TmbEntityCreditMonitoring.TmbKycCddRiskLevel), item.kycLevel },
                                { nameof(TtbEntityInfomation.TmbEntityCreditMonitoring.TmbKycLastCheckDate), DateTime.UtcNow },
                                { nameof(TtbEntityInfomation.TmbEntityCreditMonitoring.TmbKycLastCheckStatus), (int)EnumResultStatus.Success },
                                { "OperationType", "Update" }
                            }}
                        };
                }
                var resp = await _client.GetAsync(String.Format(_getUpdateEntityUri, ViewModel, item.EntityId));
                var content = await resp.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<ModelResponse>(content);
                if (response.Status != null && response.Status.ContainsKey(ResponseStatusSeverity.ERROR))
                    updateResultList.Add(new TtbUpdateEntityCls { entityRiskLevel = item, result = response });
                else
                {
                    var searchResponse = (((JArray)response.PayLoad)?.ToObject<CreditLensResponses>())?[0];
                    var jsonFromCl = JsonConvert.SerializeObject(searchResponse);
                    JObject objectFromCl = JObject.Parse(jsonFromCl);
                    foreach (var kvp in updateScheme)
                    {
                        if (!objectFromCl.TryGetValue(kvp.Key, StringComparison.InvariantCulture, out JToken token))
                            continue;

                        if (token.Type == JTokenType.Object)
                        {
                            var updatedObject = JObject.FromObject(kvp.Value);
                            //updatedObject.Add("OperationType", "Update");
                            var ogObject = objectFromCl[kvp.Key] as JObject;
                            ogObject.Merge(updatedObject, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });

                        }
                        else if (token.Type == JTokenType.Array)
                        {
                            var updatedArray = JArray.FromObject(kvp.Value);
                            var ogArray = objectFromCl[kvp.Key] as JArray;
                            ogArray.Merge(updatedArray, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
                        }
                        else
                        {
                            var property = objectFromCl.Property(kvp.Key);
                            property.Value = JToken.FromObject(kvp.Value);
                        }
                    }
                    string jsonUpdated = JsonConvert.SerializeObject(objectFromCl);
                    if (mode is EnumMode.Debug)
                    {
                        updateResultList.Add(new TtbUpdateEntityCls() { entityRiskLevel = item, result = JsonConvert.DeserializeObject<CreditLensRequest>(jsonUpdated) });
                    }
                    else
                    {
                        var updatedResult = await UpdateCreditLensService(item.EntityId, JsonConvert.DeserializeObject<CreditLensRequest>(jsonUpdated), EnumSystem.CSGW);
                        updateResultList.Add(new TtbUpdateEntityCls() { entityRiskLevel = item, result = updatedResult });
                    }
                }
            }
            return updateResultList;
        }

        public async Task<IEnumerable<TtbUpdateEntityCsi>> UpdateEntityCSICode(IEnumerable<TtbUpdateEntityScheme> updateSchemeList, EnumMode mode)
        {
            var updateResultList = new List<TtbUpdateEntityCsi>();
            foreach (var item in updateSchemeList)
            {
                var resp = await _client.GetAsync(String.Format(_getUpdateEntityUri, ViewModel, item.EntityId));
                var content = await resp.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<ModelResponse>(content);
                if (response.Status != null && response.Status.ContainsKey(ResponseStatusSeverity.ERROR))
                    updateResultList.Add(new TtbUpdateEntityCsi { updateScheme = item, result = response });
                else
                {
                    var searchResponse = (((JArray)response.PayLoad)?.ToObject<CreditLensResponses>())?[0];
                    var jsonFromCl = JsonConvert.SerializeObject(searchResponse);
                    JObject objectFromCl = JObject.Parse(jsonFromCl);
                    foreach (var kvp in item.UpdateScheme)
                    {
                        if (!objectFromCl.TryGetValue(kvp.Key, StringComparison.InvariantCulture, out JToken token))
                            continue;

                        if (token.Type == JTokenType.Object)
                        {
                            var updatedObject = JObject.FromObject(kvp.Value);
                            //updatedObject.Add("OperationType", "Update");
                            var ogObject = objectFromCl[kvp.Key] as JObject;
                            ogObject.Merge(updatedObject, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });

                        }
                        else if (token.Type == JTokenType.Array)
                        {
                            var updatedArray = JArray.FromObject(kvp.Value);
                            var ogArray = objectFromCl[kvp.Key] as JArray;
                            ogArray.Merge(updatedArray, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
                        }
                        else
                        {
                            var property = objectFromCl.Property(kvp.Key);
                            property.Value = JToken.FromObject(kvp.Value);
                        }
                    }
                    string jsonUpdated = JsonConvert.SerializeObject(objectFromCl);
                    if (mode is EnumMode.Debug)
                    {
                        updateResultList.Add(new TtbUpdateEntityCsi() { updateScheme = item, result = JsonConvert.DeserializeObject<CreditLensRequest>(jsonUpdated), IsSuccess = true });
                    }
                    else
                    {
                        var updatedResult = await UpdateCreditLensService(item.EntityId, JsonConvert.DeserializeObject<CreditLensRequest>(jsonUpdated), EnumSystem.CPSS);
                        updateResultList.Add(new TtbUpdateEntityCsi() { updateScheme = item, result = updatedResult, IsSuccess = true });
                    }
                }
            }
            return updateResultList;
        }

        public async Task<IEnumerable<TtbUpdateEntityCls>> EntityExecuteRule(List<TtbUpdateEntityCls> resUpdate, EnumMode mode)
        {
            var entityKYCSuccessList = resUpdate.Where(w => w.entityRiskLevel.IsSuccess is true);
            foreach (var row in entityKYCSuccessList)
            {
                var resp = await _client.GetAsync(String.Format(_getUpdateEntityUri, ViewModel, row.entityRiskLevel.EntityId));
                var content = await resp.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<ModelResponse>(content);
                if (response.Status != null && response.Status.ContainsKey(ResponseStatusSeverity.ERROR))
                    row.SendMailSuccess = false;
                else
                {
                    var searchResponse = (((JArray)response.PayLoad)?.ToObject<CreditLensResponses>())?[0];
                    var jsonFromCl = JsonConvert.SerializeObject(searchResponse);
                    JObject objectFromCl = JObject.Parse(jsonFromCl);
                    objectFromCl.Add("_rules", JToken.FromObject(rule));
                    string json = JsonConvert.SerializeObject(objectFromCl);
                    var updateRequest2 = JsonConvert.DeserializeObject<CreditLensRequest>("{\"payLoad\":" + json + "}");
                    row.EmailTriggerPayload = JsonConvert.SerializeObject(updateRequest2);
                    //_appLog.LogInformation($"Email Trigger EntityId is : {row.entityRiskLevel.EntityId} Body : {row.EmailTriggerPayload}");
                    row.SendMailSuccess = true;
                    if (mode is EnumMode.Release)
                    {
                        var responseMessage = await _client.PostAsync(_domainExecuteRuleUri, new StringContent(JsonConvert.SerializeObject(updateRequest2), Encoding.UTF8, "application/json"));
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            var contentRule = await responseMessage.Content.ReadAsStringAsync();
                            var modelResponse = JsonConvert.DeserializeObject<ModelResponse>(contentRule);
                            var responseRule = (((JArray)modelResponse.PayLoad)?.ToObject<CreditLensResponses>())?[0];
                            row.SendMailSuccess = (!responseRule.Error);
                        }
                        else
                        {
                            row.SendMailSuccess = false;
                        }
                    }
                }
            }
            return entityKYCSuccessList;
        }

        private async Task<TtbEntityInfomation> UpdateCreditLensService(long entityId, CreditLensRequest updateRequest, EnumSystem logicSys)
        {
            StringContent httpcontent = new(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");
            var resp = await _client.PutAsync(String.Format(_getUpdateEntityUri, ViewModel, entityId), httpcontent);
            var content = await resp.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<TtbEntityInfomation>(content);
            var payLoad = response.payLoad.FirstOrDefault();
            if (logicSys is EnumSystem.CPSS)
            {
                _appLog.LogInformation($"Update CSI Code Completed : {entityId} | TmbCsiCode : {payLoad.TmbEntityCreditMonitoring.TmbCsiCode} | TmbCsiRelatedCreditApprovalSys : {payLoad.TmbEntityCreditMonitoring.TmbCsiRelatedCreditApprovalSys} | TmbCsiLastCheckStatus : {payLoad.TmbEntityCreditMonitoring.TmbCsiLastCheckStatus} | TmbCsiLastCheckDate : {payLoad.TmbEntityCreditMonitoring.TmbCsiLastCheckDate} ");
            }
            else
            {
                _appLog.LogInformation($"Update KYC Completed : {entityId} | TmbKycCddRiskLevel :{payLoad.TmbEntityCreditMonitoring.TmbKycCddRiskLevel} | TmbKycLastCheckDate :{payLoad.TmbEntityCreditMonitoring.TmbKycLastCheckDate} | TmbKycLastCheckStatus :{payLoad.TmbEntityCreditMonitoring.TmbKycLastCheckStatus}");
            }
            return response;
        }

        public async Task<IEnumerable<TtbUpdateEntityScheme>> GenerateUpdateScheme(IEnumerable<TtbMapEntityIndex> entityMappObj)
        {
            var updateSchemeList = new List<TtbUpdateEntityScheme>();
            List<object> objectBp = new();
            int ticketId = entityMappObj.FirstOrDefault()?.Id ?? 0;
            long BorrowerId = entityMappObj.FirstOrDefault()?.EntityId ?? 0;
            foreach (var item in entityMappObj)
            {
                string csiCode = ""; var csiRelateCreditAppSysVal = "-";
                if (item.Result.listExactByCustomer.Any(a => a.resultStatusCode == EnumResultStatus.Success.ToString()))
                {
                    var objListCode = item.Result.listExactByCustomer.SelectMany(s => s.customerList).Where(w => w.listCodeC != null);
                    ProcessCSI(out csiCode, out csiRelateCreditAppSysVal, objListCode);
                }
                else if (item.Result.listExactByAddress1.Any(a => a.resultStatusCode == EnumResultStatus.Success.ToString()))
                {
                    var objCode = item.Result.listExactByAddress1.Where(w => w.listCodeC != null);
                    var objT = objCode.FirstOrDefault();
                    var listC = objT.listCodeC.Distinct();
                    //RLOS = objT.foundRLOS.ToString();
                    //SELOS = objT.foundSELOS.ToString();
                    //FDR = objT.foundFDR.ToString();
                    csiCode = string.Join(",", listC);
                }
                else if (item.Result.listExactByAddress2.Any(a => a.resultStatusCode == EnumResultStatus.Success.ToString()))
                {
                    var objCode = item.Result.listExactByAddress2.Where(w => w.listCodeC != null);
                    var objT = objCode.FirstOrDefault();
                    var listC = objT.listCodeC.Distinct();
                    //RLOS = objT.foundRLOS.ToString();
                    //SELOS = objT.foundSELOS.ToString();
                    //FDR = objT.foundFDR.ToString();
                    csiCode = string.Join(",", listC);
                }
                else if (item.Result.listExactByAddress3.Any(a => a.resultStatusCode == EnumResultStatus.Success.ToString()))
                {
                    var objCode = item.Result.listExactByAddress3.Where(w => w.listCodeC != null);
                    var objT = objCode.FirstOrDefault();
                    var listC = objT.listCodeC.Distinct();
                    //RLOS = objT.foundRLOS.ToString();
                    //SELOS = objT.foundSELOS.ToString();
                    //FDR = objT.foundFDR.ToString();
                    csiCode = string.Join(",", listC);
                }
                else if (item.Result.listExactByAddress4.Any(a => a.resultStatusCode == EnumResultStatus.Success.ToString()))
                {
                    var objCode = item.Result.listExactByAddress4.Where(w => w.listCodeC != null);
                    var objT = objCode.FirstOrDefault();
                    var listC = objT.listCodeC.Distinct();
                    //RLOS = objT.foundRLOS.ToString();
                    //SELOS = objT.foundSELOS.ToString();
                    //FDR = objT.foundFDR.ToString();
                    csiCode = string.Join(",", listC);
                }

                if (item.Result.ENTITY_ID is 1)
                {
                    Dictionary<string, object> updateScheme = new Dictionary<string, object>()
                        {
                            {
                                nameof(TtbEntityInfomation.TmbEntityCreditMonitoring), new Dictionary<string, object>()
                                {
                                    { nameof(TtbEntityInfomation.TmbEntityCreditMonitoring.TmbCsiCode), csiCode },
                                    { nameof(TtbEntityInfomation.TmbEntityCreditMonitoring.TmbCsiLastCheckStatus), (int)EnumResultStatus.Success },
                                    { nameof(TtbEntityInfomation.TmbEntityCreditMonitoring.TmbCsiLastCheckDate), DateTime.UtcNow },
                                    { nameof(TtbEntityInfomation.TmbEntityCreditMonitoring.TmbCsiRelatedCreditApprovalSys), csiRelateCreditAppSysVal },
                                    { "OperationType", "Update" }
                                }
                            }
                        };
                    updateSchemeList.Add(new TtbUpdateEntityScheme()
                    {
                        Id = item.Id,
                        EntityId = item.EntityId,
                        UpdateScheme = updateScheme
                    });
                }
                else
                {
                    if (ticketId == item.Id)
                    {
                        var mappObj = new
                        {
                            BusinessPartner = new
                            {
                                item.EntityId
                            },
                            TmbCsiCodeLongName = csiCode,
                            OperationType = "Update"
                        };
                        objectBp.Add(mappObj);
                    }
                }
                if (ticketId != item.Id)
                {
                    updateSchemeList.Add(new TtbUpdateEntityScheme()
                    {
                        Id = ticketId,
                        EntityId = BorrowerId,
                        UpdateScheme = new Dictionary<string, object>
                        {
                            {
                                nameof(TtbEntityInfomation.TmbBusinessPartnerList), objectBp
                            }
                        }
                    });
                    if (item.Result.ENTITY_ID != 1)
                    {
                        objectBp = new List<object>();
                        var mappObj = new
                        {
                            BusinessPartner = new
                            {
                                item.EntityId
                            },
                            TmbCsiCodeLongName = csiCode,
                            OperationType = "Update"
                        };
                        objectBp.Add(mappObj);
                    }
                    else
                    {
                        BorrowerId = item.EntityId;
                        objectBp = new List<object>();
                    }
                    ticketId = item.Id;
                }
            }
            if (objectBp.Any())
            {
                updateSchemeList.Add(new TtbUpdateEntityScheme()
                {
                    Id = ticketId,
                    EntityId = BorrowerId,
                    UpdateScheme = new Dictionary<string, object>
                        {
                            {
                                nameof(TtbEntityInfomation.TmbBusinessPartnerList), objectBp
                            }
                        }
                });
            }

            return updateSchemeList.OrderBy(o => o.Id);
        }

        private static void ProcessCSI(out string csiCode, out string csiRelateCreditAppSysVal, IEnumerable<CustomerList> objCode)
        {
            var objT = objCode.FirstOrDefault();
            var listC = objT.listCodeC.Distinct();
            string RLOS = objT.foundRLOS.ToString();
            string SELOS = objT.foundSELOS.ToString();
            string FDR = objT.foundFDR.ToString();
            csiCode = string.Join(",", listC);
            csiRelateCreditAppSysVal = "-";
            switch (RLOS)
            {
                case nameof(EnumBool.@false) or blank when SELOS is nameof(EnumBool.@false) or blank && FDR is nameof(EnumBool.@false) or blank:
                    csiRelateCreditAppSysVal = "-";
                    break;
                default:
                    if (RLOS is nameof(EnumBool.@true) && SELOS is nameof(EnumBool.@false) or blank && FDR is nameof(EnumBool.@false) or blank)
                    {
                        csiRelateCreditAppSysVal = "RSL";
                    }
                    else if (RLOS is nameof(EnumBool.@false) or blank && SELOS is nameof(EnumBool.@true) && FDR is nameof(EnumBool.@false) or blank)
                    {
                        csiRelateCreditAppSysVal = "SLS/CA-Web";
                    }
                    else if (RLOS is nameof(EnumBool.@false) or blank && SELOS is nameof(EnumBool.@false) or blank && FDR is nameof(EnumBool.@true))
                    {
                        csiRelateCreditAppSysVal = "FDR";
                    }
                    else if (RLOS is nameof(EnumBool.@true) && SELOS is nameof(EnumBool.@true) && FDR is nameof(EnumBool.@false) or blank)
                    {
                        csiRelateCreditAppSysVal = "RSL, SLS/CA-Web";
                    }
                    else if (RLOS is nameof(EnumBool.@true) && SELOS is nameof(EnumBool.@false) or blank && FDR is nameof(EnumBool.@true))
                    {
                        csiRelateCreditAppSysVal = "RSL, FDR";
                    }
                    else if (RLOS is nameof(EnumBool.@false) or blank && SELOS is nameof(EnumBool.@true) && FDR is nameof(EnumBool.@true))
                    {
                        csiRelateCreditAppSysVal = "SLS/CA-Web, FDR";
                    }
                    else if (RLOS is nameof(EnumBool.@true) && SELOS is nameof(EnumBool.@true) && FDR is nameof(EnumBool.@true))
                    {
                        csiRelateCreditAppSysVal = "RSL, SLS/CA-Web, FDR";
                    }
                    break;
            }
        }
    }
}
