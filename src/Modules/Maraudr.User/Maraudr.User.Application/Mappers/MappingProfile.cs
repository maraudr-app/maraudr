using Maraudr.Application.DTOs;
using Maraudr.User.Application.DTOs.Requests;
using Maraudr.User.Application.DTOs.Responses;
using Maraudr.User.Domain.Entities;

namespace Application.Mappers;

using AutoMapper;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto,User>();
            CreateMap<UpdateUserDto, User>();
        }
    }

