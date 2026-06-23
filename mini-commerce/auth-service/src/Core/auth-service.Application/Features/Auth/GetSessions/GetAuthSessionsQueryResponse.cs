using auth_service.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.GetSessions
{
    public record GetAuthSessionsQueryResponse(IReadOnlyList<AuthSessionDto> Sessions);
}
