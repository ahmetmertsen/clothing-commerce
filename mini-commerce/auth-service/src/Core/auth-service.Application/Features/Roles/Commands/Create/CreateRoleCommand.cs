using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Roles.Commands.Create
{
    public class CreateRoleCommand : IRequest<CreateRoleCommandResponse>
    {
        public string Name { get; set; } = null!;
    }
}
