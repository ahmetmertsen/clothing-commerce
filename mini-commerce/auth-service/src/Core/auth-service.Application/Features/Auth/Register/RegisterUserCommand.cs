using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.Register
{
    public record RegisterUserCommand(string FullName, string Email, string Password) : IRequest<RegisterUserCommandResponse>
    {
    }
}
