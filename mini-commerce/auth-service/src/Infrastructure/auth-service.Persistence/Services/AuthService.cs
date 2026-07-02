using auth_service.Application.Abstractions.Services;
using auth_service.Application.Abstractions.Token;
using auth_service.Application.Dtos.Auth;
using auth_service.Application.Exceptions;
using auth_service.Application.Helpers;
using auth_service.Domain.Entities;
using auth_service.Domain.Enums;
using auth_service.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Events.AuthEvents;
using Shared.Messages.Notification.Enums;
using System.Text.Json;

namespace auth_service.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AuthServiceDbContext _context;
        private readonly ITokenHandler _tokenHandler;
        private readonly IAuthSessionService _authSessionService;
        private readonly IVerificationChallengeService _verificationChallengeService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenHandler tokenHandler, IAuthSessionService authSessionService, IConfiguration configuration, ILogger<AuthService> logger, IVerificationChallengeService verificationChallengeService, AuthServiceDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenHandler = tokenHandler;
            _authSessionService = authSessionService;
            _configuration = configuration;
            _logger = logger;
            _verificationChallengeService = verificationChallengeService;
            _context = context;
        }

        public async Task<AuthTokenDto> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Login failed. Reason: {Reason}", "UserNotFound");
                throw new UnauthorizedAccesException("Kullanıcı adı veya şifre hatalı!");
            }

            await EnsureModerationStatusAsync(user);

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Login blocked. Reason: {Reason}, UserId: {UserId}", "IdentityLockout", user.Id);
                throw new UnauthorizedAccesException("Çok fazla başarısız giriş denemesi yapıldı. Lütfen daha sonra tekrar deneyin.");
            }

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed. Reason: {Reason}, UserId: {UserId}, UserName: {UserName}", "InvalidPassword", user.Id, user.Email);
                throw new UnauthorizedAccesException("Kullanıcı adı veya şifre hatalı!");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var sessionId = Guid.NewGuid();
            var refreshToken = _tokenHandler.CreateRefreshToken();
            var token = _tokenHandler.CreateAccessToken(user, roles, sessionId, refreshToken);
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays());

            await _authSessionService.CreateSessionAsync(user.Id, sessionId, Guid.NewGuid(), refreshToken, refreshTokenExpiresAt, cancellationToken);

            _logger.LogInformation("Login succeeded. UserId: {UserId}, UserName: {UserName}, RolesCount: {RolesCount}, EmailConfirmed: {EmailConfirmed}, SessionId: {SessionId}",
                user.Id,
                user.Email,
                roles.Count,
                user.EmailConfirmed,
                sessionId);

            return token;
        }


        public async Task<AuthTokenDto> RefreshTokenLoginAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var replacementSessionId = Guid.NewGuid();
            var replacementRefreshToken = _tokenHandler.CreateRefreshToken();
            var replacementExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays());

            var rotation = await _authSessionService.RotateSessionAsync(refreshToken, replacementSessionId, replacementRefreshToken, replacementExpiresAt, cancellationToken);

            var user = await _userManager.FindByIdAsync(rotation.UserId.ToString());
            if (user == null)
            {
                await _authSessionService.RevokeAllSessionsAsync(rotation.UserId, "User not found during refresh", cancellationToken);
                throw new InvalidRefreshTokenException("Refresh token geçersiz veya süresi dolmuş.");
            }

            try
            {
                await EnsureModerationStatusAsync(user);
            }
            catch
            {
                await _authSessionService.RevokeAllSessionsAsync(user.Id, "Account is not allowed to refresh", cancellationToken);
                throw;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenHandler.CreateAccessToken(user, roles, replacementSessionId, replacementRefreshToken);

            _logger.LogInformation("Refresh token login succeeded. UserId: {UserId}, UserName: {UserName}, RolesCount: {RolesCount}, SessionId: {SessionId}",
                user.Id,
                user.UserName,
                roles.Count,
                replacementSessionId);

            return token;
        }


        public async Task<ForgotPasswordResponse> ForgotPasswordResetAsync(ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            var response = new ForgotPasswordResponse
            {
                Succeeded = true,
                Message = "Mail adresi doğru ise şifre sıfırlama kodu gönderildi."
            };

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return response;
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return response;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var correlationId = Guid.NewGuid();
            var verificationCode = await _verificationChallengeService.CreateCodeAsync(user.Id, VerificationPurpose.PasswordReset, user.Email, correlationId, cancellationToken);

            var outboxToken = Guid.NewGuid();
            #region Notification Event
            NotificationRequested notificationRequested = new()
            {
                NotificationId = outboxToken,
                RecipientUserId = user.Id,
                RecipientEmail = user.Email,
                Type = NotificationType.PasswordReset,
                Channel = NotificationChannel.Email,
                IsSensitive = true,
                TemplateData = new Dictionary<string, string>
                {
                    ["full_name"] = user.FullName,
                    ["verification_code"] = verificationCode,
                    ["app_name"] = "Mini Commerce"
                },
                CorrelationId = correlationId
            };
            #endregion

            #region AuthOutbox write
            AuthOutbox authOutbox = new()
            {
                IdempotentToken = outboxToken,
                CorrelationId = correlationId,
                OccuredOn = DateTime.UtcNow,
                ProcessedDate = null,
                Status = AuthOutboxStatus.Pending,
                RetryCount = 0,
                MaxRetryCount = 5,
                IsSensitive = notificationRequested.IsSensitive,
                Payload = JsonSerializer.Serialize(notificationRequested),
                Type = notificationRequested.GetType().Name
            };

            await _context.AuthOutboxes.AddAsync(authOutbox, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            #endregion

            return response;
        }


        public async Task<MailVerifyResponse> MailVerifyAsync(MailVerifyRequest request, CancellationToken cancellationToken)
        {
            var response = new MailVerifyResponse
            {
                Succeeded = true,
                Message = "Doğrulama kodu e-posta adresinize gönderildi."
            };

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return response;
            }

            if (user.EmailConfirmed)
            {
                response.Message = "E-posta adresi zaten doğrulanmış.";
                return response;
            }

            if (user.EmailVerificationSentAt.HasValue && user.EmailVerificationSentAt.Value > DateTime.UtcNow.AddMinutes(-1))
            {
                response.Message = "Doğrulama e-postası kısa süre önce gönderildi. Lütfen tekrar denemeden önce bekleyin.";
                return response;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var correlationId = Guid.NewGuid();
            var verificationCode = await _verificationChallengeService.CreateCodeAsync(user.Id, VerificationPurpose.EmailVerification, user.Email, correlationId, cancellationToken);

            var outboxToken = Guid.NewGuid();
            #region Notification Event
            NotificationRequested notificationRequested = new()
            {
                NotificationId = outboxToken,
                RecipientUserId = user.Id,
                RecipientEmail = user.Email,
                Type = NotificationType.EmailVerification,
                Channel = NotificationChannel.Email,
                IsSensitive = true,
                TemplateData = new Dictionary<string, string>
                {
                    ["full_name"] = user.FullName,
                    ["verification_code"] = verificationCode,
                    ["app_name"] = "Mini Commerce"
                },
                CorrelationId = correlationId
            };
            #endregion

            #region AuthOutbox write
            AuthOutbox authOutbox = new()
            {
                IdempotentToken = outboxToken,
                CorrelationId = correlationId,
                OccuredOn = DateTime.UtcNow,
                ProcessedDate = null,
                Status = AuthOutboxStatus.Pending,
                RetryCount = 0,
                MaxRetryCount = 5,
                IsSensitive = notificationRequested.IsSensitive,
                Payload = JsonSerializer.Serialize(notificationRequested),
                Type = notificationRequested.GetType().Name
            };
            #endregion

            await _context.AuthOutboxes.AddAsync(authOutbox, cancellationToken);
            user.EmailVerificationSentAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Verification mail requested. UserId: {UserId}", user.Id);
            return response;
        }


        public async Task<ChangeEmailResponse> ChangeEmailAsync(ChangeEmailRequest request, CancellationToken cancellationToken)
        {
            var response = new ChangeEmailResponse
            {
                Succeeded = true,
                Message = "Yeni e-posta adresiniz uygunsa mevcut ve yeni e-posta adreslerinize doğrulama kodları gönderildi."
            };

            if (string.IsNullOrWhiteSpace(request.NewEmail))
            {
                return response;
            }

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return response;
            }

            var newEmail = request.NewEmail.Trim().ToLowerInvariant();
            var existingUser = await _userManager.FindByEmailAsync(newEmail);
            if (existingUser != null)
            {
                return response;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var correlationId = Guid.NewGuid();
            var oldEmailVerificationCode = await _verificationChallengeService.CreateCodeAsync(user.Id, VerificationPurpose.EmailChangeOld, user.Email, correlationId, cancellationToken);
            var newEmailVerificationCode = await _verificationChallengeService.CreateCodeAsync(user.Id, VerificationPurpose.EmailChangeNew, newEmail, correlationId, cancellationToken);

            var oldOutboxToken = Guid.NewGuid();
            var newOutboxToken = Guid.NewGuid();
            #region Notification Event
            NotificationRequested notificationRequestedOld = new()
            {
                NotificationId = oldOutboxToken,
                RecipientUserId = user.Id,
                RecipientEmail = user.Email,
                Type = NotificationType.EmailChangeOld,
                Channel = NotificationChannel.Email,
                IsSensitive = true,
                TemplateData = new Dictionary<string, string>
                {
                    ["full_name"] = user.FullName,
                    ["verification_code"] = oldEmailVerificationCode,
                    ["new_email"] = newEmail,
                    ["app_name"] = "Mini Commerce"
                },
                CorrelationId = correlationId
            };
            NotificationRequested notificationRequestedNew = new()
            {
                NotificationId = newOutboxToken,
                RecipientUserId = user.Id,
                RecipientEmail = newEmail,
                Type = NotificationType.EmailChangeNew,
                Channel = NotificationChannel.Email,
                IsSensitive = true,
                TemplateData = new Dictionary<string, string>
                {
                    ["full_name"] = user.FullName,
                    ["verification_code"] = newEmailVerificationCode,
                    ["app_name"] = "Mini Commerce"
                },
                CorrelationId = correlationId
            };
            #endregion

            #region AuthOutbox write
            AuthOutbox authOutboxOld = new()
            {
                IdempotentToken = oldOutboxToken,
                CorrelationId = correlationId,
                OccuredOn = DateTime.UtcNow,
                ProcessedDate = null,
                Status = AuthOutboxStatus.Pending,
                RetryCount = 0,
                MaxRetryCount = 5,
                IsSensitive = notificationRequestedOld.IsSensitive,
                Payload = JsonSerializer.Serialize(notificationRequestedOld),
                Type = notificationRequestedOld.GetType().Name
            };
            AuthOutbox authOutboxNew = new()
            {
                IdempotentToken = newOutboxToken,
                CorrelationId = correlationId,
                OccuredOn = DateTime.UtcNow,
                ProcessedDate = null,
                Status = AuthOutboxStatus.Pending,
                RetryCount = 0,
                MaxRetryCount = 5,
                IsSensitive = notificationRequestedNew.IsSensitive,
                Payload = JsonSerializer.Serialize(notificationRequestedNew),
                Type = notificationRequestedNew.GetType().Name
            };

            await _context.AuthOutboxes.AddAsync(authOutboxOld, cancellationToken);
            await _context.AuthOutboxes.AddAsync(authOutboxNew, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            #endregion

            return response;
        }


        private async Task EnsureModerationStatusAsync(User user)
        {
            if (user.Status == UserStatus.Banned)
            {
                _logger.LogWarning("Login blocked. Reason: {Reason}, UserId: {UserId}", "UserBanned", user.Id);
                throw new UnauthorizedAccesException("Bu hesap platformdan yasaklanmıştır.");
            }

            if (user.Status != UserStatus.Suspended)
            {
                return;
            }

            if (!user.SuspendedUntil.HasValue || user.SuspendedUntil.Value > DateTime.UtcNow)
            {
                _logger.LogWarning("Login blocked. Reason: {Reason}, UserId: {UserId}", "UserSuspended", user.Id);
                throw new UnauthorizedAccesException("Bu hesap geçici olarak askıya alınmıştır.");
            }

            user.Status = UserStatus.Active;
            user.SuspendedUntil = null;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new UnauthorizedAccesException("Kullanıcı hesabı güncellenemedi.");
            }
        }

        private int GetRefreshTokenExpirationDays() =>
            int.TryParse(_configuration["Token:RefreshTokenExpirationDays"], out var days) && days > 0 ? days : 30;
    }
}
