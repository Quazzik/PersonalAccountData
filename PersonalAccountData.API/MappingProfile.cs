using AutoMapper;
using PersonalAccountData.API.DTOs;
using PersonalAccountData.Core.Entities;

namespace PersonalAccountData.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<Resident, ResidentDto>().ReverseMap();
        }
    }
}
