using auth_service.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Abstractions.Services
{
    public interface IAuthSessionService
    {
        Task CreateSessionAsync(Guid userId, Guid sessionId, Guid tokenFamilyId, string refreshToken, DateTime expiresAt, CancellationToken cancellationToken);
        Task<AuthSessionRotationResult> RotateSessionAsync(string currentRefreshToken, Guid replacementSessionId, string replacementRefreshToken, DateTime replacementExpiresAt, CancellationToken cancellationToken);
        Task<bool> IsSessionActiveAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken);
        Task<IReadOnlyList<AuthSessionDto>> GetActiveSessionsAsync(Guid userId, Guid currentSessionId, CancellationToken cancellationToken);
        Task<bool> RevokeSessionAsync(Guid userId, Guid sessionId, string reason, CancellationToken cancellationToken);
        Task RevokeAllSessionsAsync(Guid userId, string reason, CancellationToken cancellationToken);
    }
}
