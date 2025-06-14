
using AutoMapper;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.API.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, Users>(); // ✅ Maps DTO to Entity
            CreateMap<Users, CreateUserDto>(); // ✅ Reverse Mapping
        }
    }
}

