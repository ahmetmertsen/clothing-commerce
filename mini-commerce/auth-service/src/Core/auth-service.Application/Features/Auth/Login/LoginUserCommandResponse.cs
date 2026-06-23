using auth_service.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.Login
{
    public record LoginUserCommandResponse(bool Succeeded, string Message, AuthTokenDto Token)
    {
    }
}
