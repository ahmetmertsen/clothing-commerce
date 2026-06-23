using auth_service.Application.Abstractions.Services;
using auth_service.Application.Dtos.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.RefreshTokenLogin
{
    public class RefreshTokenLoginCommandHandler : IRequestHandler<RefreshTokenLoginCommand, RefreshTokenLoginCommandResponse>
    {
        private readonly IAuthService _authService;

        public RefreshTokenLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<RefreshTokenLoginCommandResponse> Handle(RefreshTokenLoginCommand request, CancellationToken cancellationToken)
        {
            AuthTokenDto token = await _authService.RefreshTokenLoginAsync(request.RefreshToken, cancellationToken);
            return new RefreshTokenLoginCommandResponse { Token = token };
        }
    }
}
