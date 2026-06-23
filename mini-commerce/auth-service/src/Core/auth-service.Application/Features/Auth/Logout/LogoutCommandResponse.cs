using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.Logout
{
    public record LogoutCommandResponse(bool Succeeded, string Message);
}
