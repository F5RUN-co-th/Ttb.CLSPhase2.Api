using AutoMapper;
using CLSPhase2.Dal.Entities.CLS;
using CLSPhase2.Dal.Entities.CPSS;

namespace CLSPhase2.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TtbEntityInfomation.EntityAddress, TtbEntityInfoAusNonAus.Address>().ReverseMap();
            CreateMap<NSLL_SVC_SEARCH_BASIC_CM.ENTITYLIST, NSLL_SVC_SEARCH_BASIC_CM.ENTITYLISTS>().ReverseMap();
        }
    }
}
