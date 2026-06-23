using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.Login
{
    public record LoginUserCommand(string Email, string Password) : IRequest<LoginUserCommandResponse>
    {
    }
}
