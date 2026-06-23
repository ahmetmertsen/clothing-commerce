using auth_service.Application.Dtos.Auth;

namespace auth_service.Application.Abstractions.Services
{
    public interface IAuthService
    {
        Task<AuthTokenDto> LoginAsync(string email, string password, CancellationToken cancellationToken);
        Task<AuthTokenDto> RefreshTokenLoginAsync(string refreshToken, CancellationToken cancellationToken);
        Task<ForgotPasswordResponse> ForgotPasswordResetAsync(ForgotPasswordRequest request);
        Task<MailVerifyResponse> MailVerifyAsync(MailVerifyRequest request, CancellationToken cancellationToken);
        Task<ChangeEmailResponse> ChangeEmailAsync(ChangeEmailRequest request);
    }
}
