using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Roles.Commands.Delete
{
    public class DeleteRoleCommand : IRequest<DeleteRoleCommandResponse>
    {
        public Guid Id { get; set; }
    }
}
