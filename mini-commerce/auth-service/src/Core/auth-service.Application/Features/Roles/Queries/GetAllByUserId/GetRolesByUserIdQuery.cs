using auth_service.Application.Dtos.Role;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Roles.Queries.GetAllByUserId
{
    public class GetRolesByUserIdQuery : IRequest<List<RoleDto>>
    {
        public string UserId { get; set; }
    }
}
