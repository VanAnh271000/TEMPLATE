using Application.DTOs.Identity;
using AutoMapper;
using Domain.Entities.Identity;

namespace Application.Mappings
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<ApplicationUser, AccountDto>();
            CreateMap<UpdateProfileDto, ApplicationUser>().ReverseMap();
        }
    }
}
