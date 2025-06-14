
using AutoMapper;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.API.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>(); // ✅ Maps DTO to Entity
            CreateMap<User, CreateUserDto>(); // ✅ Reverse Mapping
        }
    }
}

