using auth_service.Application.Exceptions;
using auth_service.Domain.Entities;
using auth_service.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Common.Behaviors
{
    public class AccountStatusBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public AccountStatusBehavior(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                return await next();
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                principal.FindFirst("sub")?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccesException("Geçerli kullanıcı bilgisi bulunamadı.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new UnauthorizedAccesException("Kullanıcı hesabı bulunamadı.");
            }

            if (user.Status == UserStatus.Banned)
            {
                throw new ForbiddenException("Bu hesap platformdan yasaklanmıştır.");
            }

            if (user.Status == UserStatus.Suspended)
            {
                if (!user.SuspendedUntil.HasValue || user.SuspendedUntil.Value > DateTime.UtcNow)
                {
                    throw new ForbiddenException("Bu hesap geçici olarak askıya alınmıştır.");
                }

                user.Status = UserStatus.Active;
                user.SuspendedUntil = null;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    throw new UnauthorizedAccesException("Kullanıcı hesabı güncellenemedi.");
                }
            }

            if (!user.EmailConfirmed && request is not Application.Common.Interfaces.IAllowUnverifiedEmail)
            {
                throw new EmailVerificationRequiredException("İşleme devam etmek için e-posta adresinizi doğrulamalısınız.");
            }

            return await next();
        }
    }
}
