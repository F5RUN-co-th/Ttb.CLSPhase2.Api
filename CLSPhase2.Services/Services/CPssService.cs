using AutoMapper;
using CLSPhase2.Dal.Entities;
using CLSPhase2.Dal.Entities.CLS;
using CLSPhase2.Dal.Entities.CPSS;
using CLSPhase2.Dal.UnitOfWork;
using CLSPhase2.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Text;

namespace CLSPhase2.Services.Services
{
    public class CPssService : BaseService, ICPssService
    {
        private const string _basicCmRequestUri = "NSLL_SVC_SEARCH_BASIC_CM_REQUEST/results.json";
        private const string _basicCmResultUri = "NSLL_SVC_SEARCH_BASIC_CM_RESULT/results.json";

        private const int maximumSize = 35;
        private const int limitAddress = 4;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;
        public CPssService(IConfiguration config,
                                 IUnitOfWork unitOfWork,
                                 IMapper mapper,
                                 HttpClient httpClient,
                                 ILoggerFactory loggerFactory
                                 ) : base(unitOfWork, loggerFactory)
        {
            _client = httpClient;
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_baseSys.cpssModel.CpssUsername}:{_baseSys.cpssModel.CpssPassword}")));
            _mapper = mapper;
        }

        public async Task<IEnumerable<TempRequestBatch>> GetAllTempRequestBatch() => await _unitOfWork.CPssTempRequestBatchRepository.GetAll();

        public async Task<IEnumerable<BasicCmResult.Data>> BasicCMResult(IEnumerable<TempRequestBatch> tempRequestList, EnumMode mode)
        {
            var reqBatchResult = new BasicCmResult();
            foreach (var row in tempRequestList)
            {
                var obj = JsonConvert.DeserializeObject<NSLL_SVC_SEARCH_BASIC_CM_RESPONSE>(row.JsonDocOutboundResponse);
                var refcode = obj.Output.FirstOrDefault()?.referenceCode;
                var request = new NSLL_SVC_SEARCH_BASIC_CM_RESULT_REQUEST()
                {
                    Input = new Input()
                    {
                        data = new List<Datum>
                        {
                            new Datum()
                            {
                                REQUEST_SYSTEM = obj.Output.FirstOrDefault().REQUEST_SYSTEM,
                                REFERENCE_CODE = refcode,
                                APP_NUMBER = obj.Output.FirstOrDefault().APP_NUMBER,
                                ENTITY_ID = obj.Output.FirstOrDefault().ENTITY_LIST.Select(int.Parse).ToList(),
                                USER_ID = _baseSys.UserId
                            }
                        }
                    }
                };
                if (mode == EnumMode.Debug)
                {
                    var read = obj.Output.FirstOrDefault();
                    reqBatchResult.data.Add(new BasicCmResult.Data
                    {
                        Id = row.Id,
                        outbound = row.JsonDocOutboundRequest,
                        inbound = new NSLL_SVC_SEARCH_BASIC_CM_RESULT()
                        {
                            Output = new List<Output>()
                            {
                                new Output()
                                {
                                    APP_NUMBER = read.APP_NUMBER,
                                    REQUEST_SYSTEM = read.REQUEST_SYSTEM,
                                    REFERENCE_CODE = read.referenceCode,
                                    resultStatusDesc = nameof(EnumResultStatus.Success),
                                    resultStatusCode = $"{(int)HttpStatusCode.OK}",
                                    TOTAL_ENTITY = int.Parse(read.TOTAL_ENTITY),
                                    ENTITY_LIST = read.ENTITY_LIST.Select(rows => new ENTITYLIST()
                                        {
                                            ENTITY_SK =$"SLS_WEB{read.referenceCode}{rows}",
                                            ENTITY_ID =int.Parse(rows),
                                            resultStatusDesc = nameof(EnumResultStatus.Success),
                                            resultStatusCode = $"{(int)HttpStatusCode.OK}",
                                            listExactByCustomer = new List<ListExactByCustomer>(){ new ListExactByCustomer() { resultStatusCode= $"{(int)HttpStatusCode.OK}", resultStatusDesc = nameof(EnumResultStatus.Success) } },
                                            listExactByAddress1 = new List<ListExactByAddress1>(){ new ListExactByAddress1() { resultStatusCode= $"{(int)HttpStatusCode.NotModified}", resultStatusDesc = "Data Not Found" } },
                                            listExactByAddress2 = new List<ListExactByAddress2>(){ new ListExactByAddress2() { resultStatusCode= $"{(int)HttpStatusCode.NotModified}", resultStatusDesc = "Data Not Found" } },
                                            listExactByAddress3 = new List<ListExactByAddress3>(){ new ListExactByAddress3() { resultStatusCode= $"{(int)HttpStatusCode.NotModified}", resultStatusDesc = "Data Not Found" } },
                                            listExactByAddress4 = new List<ListExactByAddress4>(){ new ListExactByAddress4() { resultStatusCode= $"{(int)HttpStatusCode.NotModified}", resultStatusDesc = "Data Not Found" } },
                                        }
                                    ).ToList(),
                                    user_fields = obj.Output.Select(s=>s.REQUEST_SYSTEM).ToList()
                                }
                            }
                        },
                        IsSuccess = true,
                        referenceCode = refcode
                    });
                }
                else
                {
                    var resp = await _client.PostAsync(_baseSys.cpssModel.CpssInboundUrl, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                    if (resp.IsSuccessStatusCode)
                    {
                        var content = await resp.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<NSLL_SVC_SEARCH_BASIC_CM_RESULT>(content);
                        var resultStatusCode = EnumResultStatus.Success == (EnumResultStatus)Enum.Parse(typeof(EnumResultStatus), response.Output.FirstOrDefault().resultStatusCode);
                        if (resultStatusCode)
                        {
                            reqBatchResult.data.Add(new BasicCmResult.Data
                            {
                                Id = row.Id,
                                outbound = row.JsonDocOutboundRequest,
                                inbound = response,
                                IsSuccess = resultStatusCode,
                                referenceCode = refcode
                            });
                        }
                    }
                    else
                        break;
                }
            }
            return reqBatchResult.data;
        }

        public IEnumerable<TtbMapEntityIndex> MappingEntityIndex(IEnumerable<BasicCmResult.Data> entityIndexList)
        {
            var mappObj = new List<TtbMapEntityIndex>();
            foreach (var item in entityIndexList)
            {
                var inB = item.inbound.Output.Where(w => w.REFERENCE_CODE == item.referenceCode).SelectMany(s => s.ENTITY_LIST);
                var outbound = JsonConvert.DeserializeObject<NSLL_SVC_SEARCH_BASIC_CM_REQUEST<NSLL_SVC_SEARCH_BASIC_CM.ENTITYLISTS>>(item.outbound);
                var outB = outbound.Input.data.SelectMany(s => s.ENTITY_LIST);
                var qury = outB.GroupJoin(inB,
                  src => src.ENTITY_ID,
                  d => d.ENTITY_ID,
                  (src, des) => new { outB = src, inB = des, detial = des.FirstOrDefault() })
                  .Select(s => new TtbMapEntityIndex
                  {
                      Id = item.Id,
                      ReferenceCode = item.referenceCode,
                      EntityId = s.outB.Id,
                      Result = s.detial,
                  });
                mappObj.AddRange(qury);
            }
            return mappObj;
        }

        public async Task<IEnumerable<BasicCmRequestBatch.Data>> BasicCMRequest(BasicCmRequestBatch basicCMRequestList, EnumMode mode)
        {
            foreach (var row in basicCMRequestList.data)
            {
                if (mode == EnumMode.Debug)
                {
                    Random random = new();
                    string RandomString(int length)
                    {
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        return new string(Enumerable.Repeat(chars, length)
                            .Select(s => s[random.Next(s.Length)]).ToArray());
                    }
                    row.IsSuccess = true;
                    var jConvert = JsonConvert.DeserializeObject<NSLL_SVC_SEARCH_BASIC_CM_REQUEST<NSLL_SVC_SEARCH_BASIC_CM.ENTITYLIST>>(row.request);
                    var data = jConvert.Input.data.FirstOrDefault();
                    row.response = new NSLL_SVC_SEARCH_BASIC_CM_RESPONSE()
                    {
                        Output = new List<Outputs>()
                        {
                            new Outputs()
                            {
                                 resultStatus = $"{(int)HttpStatusCode.OK}",
                                 resultStatusDesc = nameof(EnumResultStatus.Success),
                                 APP_NUMBER = $"{random.Next(1000,9999)}",
                                 referenceCode = $"SLS_WEB{RandomString(40)}",
                                 REQUEST_SYSTEM ="SLS_WEB",
                                 TOTAL_ENTITY = data.TOTAL_ENTITY.ToString(),
                                 ENTITY_LIST = data.ENTITY_LIST.Select(s=>s.ENTITY_ID.ToString()).ToList(),
                                 user_fields = jConvert.Input.data.Select(s=>s.REQUEST_SYSTEM).ToList()
                            }
                        }
                    };
                }
                else
                {
                    var resp = await _client.PostAsync(_baseSys.cpssModel.CpssOutboundUrl, new StringContent(row.request, Encoding.UTF8, "application/json"));
                    row.IsSuccess = resp.IsSuccessStatusCode;
                    if (row.IsSuccess)
                    {
                        var content = await resp.Content.ReadAsStringAsync();
                        row.response = JsonConvert.DeserializeObject<NSLL_SVC_SEARCH_BASIC_CM_RESPONSE>(content);
                    }
                    else
                        break;
                }
            }
            return basicCMRequestList.data;
        }

        public async Task<TempRequestBatch> CreateTempRequestBatch(TempRequestBatch resultRequest)
        {
            var dataStore = new Dictionary<string, object>
            {
                { $"\"{nameof(TempRequestBatch.ReferenceCode)}\"", resultRequest.ReferenceCode },
                { $"\"{nameof(TempRequestBatch.JsonDocOutboundRequest)}\"", resultRequest.JsonDocOutboundRequest },
                { $"\"{nameof(TempRequestBatch.JsonDocOutboundResponse)}\"", resultRequest.JsonDocOutboundResponse },
                { $"\"{nameof(TempRequestBatch.CreatedAt)}\"", DateOnly.FromDateTime(DateTime.Now) }
            };

            var bathId = await _unitOfWork.CPssTempRequestBatchRepository.Create(dataStore);

            var tempRequestBatch = await _unitOfWork.CPssTempRequestBatchRepository.Get(bathId);
            return tempRequestBatch;
        }

        public async Task<IEnumerable<TempRequestBatch>> CreateTempRequestBatch(IEnumerable<TempRequestBatch> resultRequest)
        {
            var data = resultRequest.Select(s => new Dictionary<string, object>
            {
                    { $"\"{nameof(s.ReferenceCode)}\"", s.ReferenceCode },
                    { $"\"{nameof(s.JsonDocOutboundRequest)}\"", s.JsonDocOutboundRequest },
                    { $"\"{nameof(s.JsonDocOutboundResponse)}\"", s.JsonDocOutboundResponse },
                    { $"\"{nameof(s.CreatedAt)}\"", DateOnly.FromDateTime(DateTime.Now) }
            });
            var dataStore = new Dictionary<string[], IEnumerable<Dictionary<string, object>>>
            {
                {
                    new string[] { $"\"{nameof(TempRequestBatch.ReferenceCode)}\"", $"\"{nameof(TempRequestBatch.JsonDocOutboundRequest)}\"", $"\"{nameof(TempRequestBatch.JsonDocOutboundResponse)}\"", $"\"{nameof(TempRequestBatch.CreatedAt)}\"" },
                    data
                }
            };
            var rowsAffected = await _unitOfWork.CPssTempRequestBatchRepository.Create(dataStore);
            if (rowsAffected > 0)
            {
                return await _unitOfWork.CPssTempRequestBatchRepository.GetTempRequestBatchByItemList(dataStore);
            }
            return null;
        }

        public async Task<BasicCmRequestBatch> GenerateRequest(IEnumerable<TtbEntityInfoAusNonAus.TtbEntityInformation> entityList, EnumBasicCmSrchPatterns srchPatterns)
        {
            var sequenceList = entityList.Select((s, i) => new { key = s, index = (i + 1) });

            var reqBatch = new BasicCmRequestBatch();

            var newPartition = sequenceList.Chunk(maximumSize);
            foreach (var entitySection in newPartition)
            {
                var outbound = entitySection.Select(s =>
                {
                    var c = new NSLL_SVC_SEARCH_BASIC_CM.CustomerList();
                    c.customerIndex = 1;
                    c.id = s.key.TmbIdentificationId;
                    if (s.key.EntityType is "C")
                    {
                        c.companyNameEN = s.key.TmbBorrowerFullNameEn;
                        c.companyNameTH = s.key.TmbBorrowerFullNameTh;
                        c.customerType = "Juristic";
                        //obj.idType = "RegistrationId";
                    }
                    else
                    {
                        c.customerType = "Individual";
                        //obj.firstNameEN = s.key.TmbBorrowerNameEn;
                        c.firstNameTH = s.key.TmbBorrowerNameTh;
                        //obj.lastNameEN = s.key.TmbBorrowerSurnameEn;
                        c.lastNameTH = s.key.TmbBorrowerSurnameTh;
                        //obj.idType = "CitizenId";
                    }
                    var obj = new NSLL_SVC_SEARCH_BASIC_CM.ENTITYLISTS();
                    obj.ENTITY_ID = s.index;
                    obj.Id = s.key.EntityId;
                    obj.customerList = new List<NSLL_SVC_SEARCH_BASIC_CM.CustomerList> { c };
                    if (srchPatterns is EnumBasicCmSrchPatterns.AddressFlag)
                    {
                        var addrObjMapping = s.key.Address.Take(limitAddress).Select((s, i) => new
                        {
                            key = "searchAddressFlag" + (i + 1),
                            houseNo = s.TmbNo ?? "",
                            district = s.TmbDistrict ?? "",
                            subdistrict = s.TmbSubDistrict ?? "",
                            province = s.TmbProvince ?? "",
                            zipcode = s.TmbPostalCode ?? ""
                        });
                        foreach (var addr in addrObjMapping)
                        {
                            if (addr.houseNo == "" || addr.district == "" || addr.subdistrict == "" || addr.province == "" || addr.zipcode == "")
                                break;
                            switch (addr.key)
                            {
                                case nameof(obj.searchAddressFlag1):
                                    obj.searchAddressFlag1 = true;
                                    obj.houseNo1 = addr.houseNo;
                                    obj.district1 = addr.district;
                                    obj.subdistrict1 = addr.subdistrict;
                                    obj.province1 = addr.province;
                                    obj.zipcode1 = addr.zipcode;
                                    break;
                                case nameof(obj.searchAddressFlag2):
                                    obj.searchAddressFlag2 = true;
                                    obj.houseNo2 = addr.houseNo;
                                    obj.district2 = addr.district;
                                    obj.subdistrict2 = addr.subdistrict;
                                    obj.province2 = addr.province;
                                    obj.zipcode2 = addr.zipcode;
                                    break;
                                case nameof(obj.searchAddressFlag3):
                                    obj.searchAddressFlag3 = true;
                                    obj.houseNo3 = addr.houseNo;
                                    obj.district3 = addr.district;
                                    obj.subdistrict3 = addr.subdistrict;
                                    obj.province3 = addr.province;
                                    obj.zipcode3 = addr.zipcode;
                                    break;
                                case nameof(obj.searchAddressFlag4):
                                    obj.searchAddressFlag4 = true;
                                    obj.houseNo4 = addr.houseNo;
                                    obj.district4 = addr.district;
                                    obj.subdistrict4 = addr.subdistrict;
                                    obj.province4 = addr.province;
                                    obj.zipcode4 = addr.zipcode;
                                    break;
                            }
                        }
                    }
                    return obj;
                });
                var request = outbound.Select(_mapper.Map<NSLL_SVC_SEARCH_BASIC_CM.ENTITYLIST>);
                reqBatch.data.Add(new BasicCmRequestBatch.Data
                {
                    request = JsonConvert.SerializeObject(new NSLL_SVC_SEARCH_BASIC_CM_REQUEST<NSLL_SVC_SEARCH_BASIC_CM.ENTITYLIST>(_baseSys.AppId, _baseSys.UserId, entitySection.Length, request)),
                    outbound = JsonConvert.SerializeObject(new NSLL_SVC_SEARCH_BASIC_CM_REQUEST<NSLL_SVC_SEARCH_BASIC_CM.ENTITYLISTS>(_baseSys.AppId, _baseSys.UserId, entitySection.Length, outbound))
                });
            }
            return reqBatch;
        }

        public async Task<bool> DeleteTempRequestBatch(IEnumerable<int> objList) => await _unitOfWork.CPssTempRequestBatchRepository.Delete(objList.Select(s => new { idList = s }).ToArray());
    }
}