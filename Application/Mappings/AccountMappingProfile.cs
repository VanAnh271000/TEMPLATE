using Application.DTOs.Identity;
using AutoMapper;
using Domain.Entities.Identity;

namespace Application.Mappings
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<ApplicationUser, AccountDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
            CreateMap<UpdateProfileDto, ApplicationUser>().ReverseMap();
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<ApplicationRole, RoleDto>()
                .ForMember(dest => dest.Permissions, opt => opt.Ignore())
                .ForMember(dest => dest.Accounts, opt => opt.Ignore());

        }
    }
}
