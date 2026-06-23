using auth_service.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.GetSessions
{
    public class GetAuthSessionsQueryHandler : IRequestHandler<GetAuthSessionsQuery, GetAuthSessionsQueryResponse>
    {
        private readonly IAuthSessionService _authSessionService;

        public GetAuthSessionsQueryHandler(IAuthSessionService authSessionService)
        {
            _authSessionService = authSessionService;
        }

        public async Task<GetAuthSessionsQueryResponse> Handle(GetAuthSessionsQuery request, CancellationToken cancellationToken)
        {
            var sessions = await _authSessionService.GetActiveSessionsAsync(request.UserId, request.CurrentSessionId, cancellationToken);

            return new GetAuthSessionsQueryResponse(sessions);
        }
    }
}
