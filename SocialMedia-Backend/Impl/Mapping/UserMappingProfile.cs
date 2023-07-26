using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SocialMedia_Backend.Model.DTO.Auth.Request;

namespace SocialMedia_Backend.Impl.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<RegisterationRequest, IdentityUser>();
        }
    }
}
