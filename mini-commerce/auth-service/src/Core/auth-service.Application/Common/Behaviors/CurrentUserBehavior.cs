using auth_service.Application.Common.Interfaces;
using auth_service.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Common.Behaviors
{
    public class CurrentUserBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : class
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserBehavior(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is ICurrentUserRequest || request is ICurrentSessionRequest)
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user?.Identity?.IsAuthenticated != true)
                {
                    throw new UnauthorizedAccesException("Kullanıcı doğrulanamadı.");
                }

                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                    user.FindFirst("sub")?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    throw new UnauthorizedAccesException("Geçerli kullanıcı bilgisi bulunamadı.");
                }

                if (request is ICurrentUserRequest currentUserRequest)
                {
                    currentUserRequest.UserId = userId;
                }

                if (request is ICurrentSessionRequest currentSessionRequest)
                {
                    var sessionIdClaim = user.FindFirst("sid")?.Value ?? user.FindFirst(ClaimTypes.Sid)?.Value;
                    if (!Guid.TryParse(sessionIdClaim, out var sessionId))
                    {
                        throw new UnauthorizedAccesException("Geçerli oturum bilgisi bulunamadı.");
                    }

                    currentSessionRequest.CurrentSessionId = sessionId;
                }
            }
            return await next();
        }
    }
}
