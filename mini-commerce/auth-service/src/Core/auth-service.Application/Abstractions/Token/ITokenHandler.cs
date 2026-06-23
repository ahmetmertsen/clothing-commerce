using auth_service.Application.Dtos.Auth;
using auth_service.Domain.Entities;

namespace auth_service.Application.Abstractions.Token
{
    public interface ITokenHandler
    {
        public AuthTokenDto CreateAccessToken(User user, IList<string> roles, Guid sessionId, string refreshToken);
        public string CreateRefreshToken();
    }
}
