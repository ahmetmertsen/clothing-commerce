using auth_service.Application.Dtos.Role;
using auth_service.Application.Features.Roles.Commands.Create;
using auth_service.Application.Features.Roles.Commands.Update;
using auth_service.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Mapping
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<CreateRoleCommand, Role>();
            CreateMap<UpdateRoleCommand, Role>();

            CreateMap<Role, RoleDto>();
        }
    }
}
