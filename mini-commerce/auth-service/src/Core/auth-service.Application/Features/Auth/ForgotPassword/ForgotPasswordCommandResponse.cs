using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.ForgotPassword
{
    public record ForgotPasswordCommandResponse(bool Succeeded, string Message)
    {
    }
}
