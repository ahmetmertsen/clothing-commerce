using auth_service.Application.Abstractions.Services;
using auth_service.Application.Common.Consts;
using auth_service.Application.Dtos.User;
using auth_service.Application.Exceptions;
using auth_service.Application.Helpers;
using auth_service.Domain.Entities;
using auth_service.Domain.Enums;
using auth_service.Persistence.Context;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Events.AuthEvents;
using Shared.Messages.Notification.Enums;
using System.Text.Json;
using System.Threading;

namespace auth_service.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly AuthServiceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthSessionService _authSessionService;
        private readonly IVerificationChallengeService _verificationChallengeService;

        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper, AuthServiceDbContext context, IAuthSessionService authSessionService, IVerificationChallengeService verificationChallengeService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;
            _authSessionService = authSessionService;
            _verificationChallengeService = verificationChallengeService;
        }

        public async Task<RegisterUserResponseDto> RegisterAsync(RegisterUserRequestDto request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                throw new RegisterFailedException("Email kayıtlar arasında mevcut!");
            }

            if (!await _roleManager.RoleExistsAsync(RoleConstants.User))
            {
                throw new RegisterFailedException("Kayıt tamamlanamadı. User rolü sistemde tanımlı değil.");
            }

            user = _mapper.Map<User>(request);
            var email = request.Email.Trim().ToLowerInvariant();
            user.Email = email;
            user.UserName = email;

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, RoleConstants.User);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);

                    throw new RegisterFailedException("Kullanıcı rolü atanamadığı için kayıt tamamlanamadı. User rolünün tanımlı olduğundan emin olun.");

                }

                var correlationId = Guid.NewGuid();
                var verificationCode = await _verificationChallengeService.CreateCodeAsync(user.Id, VerificationPurpose.EmailVerification, user.Email!, correlationId, cancellationToken);

                var notificationOutboxToken = Guid.NewGuid();
                var customerOutboxToken = Guid.NewGuid();

                #region Notification Event
                NotificationRequested notificationRequested = new()
                {
                    NotificationId = notificationOutboxToken,
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
                    CorrelationId = correlationId,
                    OccurredAt = DateTime.UtcNow
                };
                #endregion

                #region Customer Event
                AuthUserRegisteredEvent authUserRegisteredEvent = new()
                {
                    EventId = customerOutboxToken,
                    AuthUserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    CorrelationId = correlationId,
                    OccurredAt = DateTime.UtcNow,
                };

                #endregion

                #region AuthOutbox write
                var notificationOutbox = CreateAuthOutboxMessage(notificationOutboxToken, correlationId, notificationRequested, notificationRequested.IsSensitive);
                var customerOutbox = CreateAuthOutboxMessage(customerOutboxToken, correlationId, authUserRegisteredEvent, isSensitive: false);

                await _context.AuthOutboxes.AddRangeAsync(new[] { notificationOutbox, customerOutbox }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                #endregion

                user.EmailVerificationSentAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);


                return new RegisterUserResponseDto
                {
                    Succeeded = true,
                    Message = "Kullanıcı başarıyla kaydedildi. E-posta adresinizi doğrulamanız gerekiyor.",
                    UserId = user.Id
                };
            }
            else
            {
                throw new RegisterFailedException("Kayıt sırasında hata oluştu");
            }
        }


        public async Task<UpdateUserPasswordResponse> UpdatePasswordAsync(UpdateUserPasswordRequest request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new PasswordChangeFailedException("Kullanıcının e-posta adresi bulunamadı.");
            }
            if (string.IsNullOrWhiteSpace(request.VerificationCode))
            {
                throw new PasswordChangeFailedException("Şifre sıfırlama kodu geçersiz.");
            }
            if (!request.newPassword.Equals(request.newPasswordConfirmed))
            {
                throw new PasswordChangeFailedException("Şifreler uyuşmuyor.");
            }

            await _verificationChallengeService.ValidateCodeAsync(user.Id, VerificationPurpose.PasswordReset, user.Email, request.VerificationCode, cancellationToken);
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, request.newPassword);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                user.LastPasswordChangedDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                await _authSessionService.RevokeAllSessionsAsync(user.Id, "Password changed", cancellationToken);

                return new UpdateUserPasswordResponse
                {
                    Succeeded = true,
                    Message = "Şifre başarılı bir şekilde güncellenmiştir."
                };
            }
            else
            {
                throw new PasswordChangeFailedException("Şifre oluşturulurken hata oluştu...");
            }
        }


        public async Task<UpdateUserMailVerifyResponse> UpdateUserMailVerify(UpdateUserMailVerifyRequest request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }
            if (user.EmailConfirmed == true)
            {
                return new UpdateUserMailVerifyResponse
                {
                    Succeeded = true,
                    Message = "E-posta adresi zaten doğrulanmış."
                };
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new BadRequestException("Kullanıcının e-posta adresi bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(request.VerificationCode))
            {
                throw new BadRequestException("Doğrulama kodu geçersiz.");
            }

            await _verificationChallengeService.ValidateCodeAsync(user.Id, VerificationPurpose.EmailVerification, user.Email, request.VerificationCode, cancellationToken);
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                user.EmailVerificationSentAt = null;
                await _userManager.UpdateAsync(user);

                return new UpdateUserMailVerifyResponse
                {
                    Succeeded = true,
                    Message = "E-posta adresiniz başarıyla doğrulandı."
                };
            }
            else
            {
                throw new MailVerifyFailedException("E-posta doğrulama başarısız. Bağlantı süresi dolmuş olabilir.");
            }
        }


        public async Task<UpdateUserEmailResponse> UpdateUserEmailAsync(UpdateUserEmailRequest request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new BadRequestException("Kullanıcının mevcut e-posta adresi bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(request.OldEmailVerificationCode) || string.IsNullOrWhiteSpace(request.NewEmailVerificationCode))
            {
                throw new BadRequestException("Doğrulama kodları geçersiz.");
            }

            var newEmail = request.NewEmail.Trim().ToLowerInvariant();
            var existingUser = await _userManager.FindByEmailAsync(newEmail);
            if (existingUser != null)
            {
                throw new ChangeEmailFailedException("Email güncellenirken hata oluştu...");
            }

            var oldEmail = user.Email;
            await _verificationChallengeService.ValidateEmailChangeCodesAsync(user.Id, oldEmail, newEmail, request.OldEmailVerificationCode, request.NewEmailVerificationCode, cancellationToken);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            string token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            IdentityResult result = await _userManager.ChangeEmailAsync(user, newEmail, token);

            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                await _userManager.UpdateAsync(user);
                await _authSessionService.RevokeAllSessionsAsync(user.Id, "Email changed", cancellationToken);

                var correlationId = Guid.NewGuid();
                var emailChangedOutboxToken = Guid.NewGuid();
                AuthUserEmailChangedEvent authUserEmailChangedEvent = new()
                {
                    EventId = emailChangedOutboxToken,
                    AuthUserId = user.Id,
                    OldEmail = oldEmail!,
                    NewEmail = newEmail,
                    CorrelationId = correlationId,
                    OccurredAt = DateTime.UtcNow
                };

                var emailChangedOutbox = CreateAuthOutboxMessage(emailChangedOutboxToken, correlationId, authUserEmailChangedEvent, isSensitive: false);
                await _context.AuthOutboxes.AddAsync(emailChangedOutbox, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new UpdateUserEmailResponse
                {
                    Succeeded = true,
                    Message = "Email başarılı bir şekilde güncellenmiştir."
                };
            }
            else
            {
                throw new ChangeEmailFailedException("Email güncellenirken hata oluştu...");
            }
        }


        public async Task<(List<AdminUserDto> Items, int TotalCount)> GetPagedUsersAsync(int page, int size, string? search, UserStatus? status, bool? emailConfirmed, CancellationToken cancellationToken)
        {
            var query = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();
                var pattern = $"%{keyword}%";

                query = query.Where(user =>
                    EF.Functions.Like(user.FullName, pattern) ||
                    (user.Email != null && EF.Functions.Like(user.Email, pattern)));
            }

            if (status.HasValue)
            {
                query = query.Where(user => user.Status == status.Value);
            }

            if (emailConfirmed.HasValue)
            {
                query = query.Where(user => user.EmailConfirmed == emailConfirmed.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query.OrderBy(user => user.Email).ThenBy(user => user.Id).Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
            var items = new List<AdminUserDto>(users.Count);

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                items.Add(new AdminUserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    EmailConfirmed = user.EmailConfirmed,
                    Status = user.Status,
                    SuspendedUntil = user.SuspendedUntil,
                    LockoutEnd = user.LockoutEnd,
                    IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow,
                    Roles = roles.OrderBy(role => role).ToArray()
                });
            }

            return (items, totalCount);
        }


        public async Task<UserDto> GetUserById(Guid userId)
        {
            var user = await ProjectUsers().FirstOrDefaultAsync(item => item.Id == userId);
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }

            return user;
        }


        public async Task AssignRoleToUserAsync(Guid actorUserId, Guid targetUserId, string[] roles, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(targetUserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }

            var requestedRoleNames = (roles ?? Array.Empty<string>()).Where(role => !string.IsNullOrWhiteSpace(role)).Select(role => role.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (requestedRoleNames.Length == 0)
            {
                throw new BadRequestException("Kullanıcıya en az bir rol atanmalıdır.");
            }

            var availableRoleNames = await _roleManager.Roles.AsNoTracking().Where(role => role.Name != null).Select(role => role.Name!).ToListAsync(cancellationToken);
            var invalidRoles = requestedRoleNames.Where(role => !availableRoleNames.Contains(role, StringComparer.OrdinalIgnoreCase)).ToArray();
            if (invalidRoles.Length > 0)
            {
                throw new BadRequestException($"Tanımlı olmayan roller gönderildi: {string.Join(", ", invalidRoles)}");
            }

            var resolvedRoles = requestedRoleNames.Select(role => availableRoleNames.First(availableRole => string.Equals(availableRole, role, StringComparison.OrdinalIgnoreCase))).ToArray();
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removesAdminRole = currentRoles.Contains(RoleConstants.Admin, StringComparer.OrdinalIgnoreCase) && !resolvedRoles.Contains(RoleConstants.Admin, StringComparer.OrdinalIgnoreCase);

            if (actorUserId == targetUserId && removesAdminRole)
            {
                throw new BadRequestException("Kendi Admin rolünüzü kaldıramazsınız.");
            }

            if (removesAdminRole)
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync(RoleConstants.Admin);
                if (adminUsers.Count <= 1)
                {
                    throw new BadRequestException("Sistemde en az bir Admin kullanıcısı bulunmalıdır.");
                }
            }

            var rolesToAdd = resolvedRoles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToArray();
            var rolesToRemove = currentRoles.Except(resolvedRoles, StringComparer.OrdinalIgnoreCase).ToArray();
            if (rolesToAdd.Length == 0 && rolesToRemove.Length == 0)
            {
                return;
            }

            if (rolesToAdd.Length > 0)
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    throw new BadRequestException($"Roller atanamadı: {GetIdentityErrors(addResult)}");
                }
            }

            if (rolesToRemove.Length > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    if (rolesToAdd.Length > 0)
                    {
                        await _userManager.RemoveFromRolesAsync(user, rolesToAdd);
                    }

                    throw new BadRequestException($"Mevcut roller kaldırılamadı: {GetIdentityErrors(removeResult)}");
                }
            }

            var securityStampResult = await _userManager.UpdateSecurityStampAsync(user);
            if (!securityStampResult.Succeeded)
            {
                throw new BadRequestException($"Rol değişikliği güvenlik bilgisine yansıtılamadı: {GetIdentityErrors(securityStampResult)}");
            }

            await _authSessionService.RevokeAllSessionsAsync(user.Id, "Roles changed", cancellationToken);
        }

        
        public async Task<string[]> GetRolesToUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles.ToArray();
        }

       

        private IQueryable<UserDto> ProjectUsers() => _userManager.Users.AsNoTracking().Select(user => new UserDto
        {
            Id = user.Id,
            FullName = user.FullName
        });

        private static AuthOutbox CreateAuthOutboxMessage<TMessage>(Guid idempotentToken, Guid correlationId, TMessage message, bool isSensitive)
            where TMessage : class
        {
            return new AuthOutbox
            {
                IdempotentToken = idempotentToken,
                CorrelationId = correlationId,
                OccuredOn = DateTime.UtcNow,
                Status = AuthOutboxStatus.Pending,
                RetryCount = 0,
                MaxRetryCount = 5,
                IsSensitive = isSensitive,
                Payload = JsonSerializer.Serialize(message),
                Type = typeof(TMessage).Name
            };
        }

        private static string GetIdentityErrors(IdentityResult result) => string.Join(", ", result.Errors.Select(error => error.Description));



    }
}
