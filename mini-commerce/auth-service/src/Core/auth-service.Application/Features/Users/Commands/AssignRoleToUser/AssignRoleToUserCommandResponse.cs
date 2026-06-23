using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Commands.AssignRoleToUser
{
    public class AssignRoleToUserCommandResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = null!;
    }
}
