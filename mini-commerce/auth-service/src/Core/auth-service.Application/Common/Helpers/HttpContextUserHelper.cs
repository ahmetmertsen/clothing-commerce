using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace auth_service.Application.Common.Helpers
{
    public static class HttpContextUserHelper
    {
        public static int? GetUserId(HttpContext? httpContext)
        {
            var user = httpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? user.FindFirst("sub")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
