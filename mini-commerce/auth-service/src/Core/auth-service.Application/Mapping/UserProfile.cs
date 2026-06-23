using auth_service.Application.Dtos.User;
using auth_service.Application.Features.Auth.Register;
using auth_service.Application.Features.Users.Commands.Update.UpdateEmail;
using auth_service.Application.Features.Users.Commands.Update.UpdatePassword;
using auth_service.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //Register
            CreateMap<RegisterUserCommand, User>();
            CreateMap<RegisterUserCommand, RegisterUserRequestDto>();
            CreateMap<RegisterUserRequestDto, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLowerInvariant()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email.Trim().ToLowerInvariant()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<User, RegisterUserCommandResponse>();

            CreateMap<UpdateUserEmailCommand, User>();
            CreateMap<UpdateUserPasswordCommand, User>();

            //Dto
            CreateMap<User, UserDto>()
                .ForMember(destination => destination.FullName, options => options.MapFrom(source => source.FullName));
        }
    }
}
