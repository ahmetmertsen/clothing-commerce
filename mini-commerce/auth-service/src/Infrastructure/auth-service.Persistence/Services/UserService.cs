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
using Microsoft.Extensions.Logging;

namespace auth_service.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly AuthServiceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthSessionService _authSessionService;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper, AuthServiceDbContext context, IAuthSessionService authSessionService, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;
            _authSessionService = authSessionService;
            _logger = logger;
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

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, RoleConstants.User);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);

                    throw new RegisterFailedException("Kullanıcı rolü atanamadığı için kayıt tamamlanamadı. User rolünün tanımlı olduğundan emin olun.");

                }

                var message = "Kullanıcı başarıyla kaydedildi. E-posta adresinizi doğrulamanız gerekiyor.";
                try
                {
                    var emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    /*
                    await _mailService.SendVerifyMailAsync(user.Email!, user.FullName, user.Id, emailConfirmToken.UrlEncode());
                    */

                    user.EmailVerificationSentAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }
                catch (Exception exception)
                {
                    message = "Kullanıcı başarıyla kaydedildi ancak doğrulama e-postası gönderilemedi. Tekrar gönderme işlemini kullanabilirsiniz.";

                    _logger.LogWarning(exception, "Registration verification mail could not be sent. UserId: {UserId}", user.Id);

                }

                return new RegisterUserResponseDto
                {
                    Succeeded = true,
                    Message = message,
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
            User user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }
            if (string.IsNullOrWhiteSpace(request.ResetToken))
            {
                throw new PasswordChangeFailedException("Şifre sıfırlama bağlantısı geçersiz.");
            }
            if (!request.newPassword.Equals(request.newPasswordConfirmed))
            {
                throw new PasswordChangeFailedException("Şifreler uyuşmuyor.");
            }
            string resetToken = request.ResetToken.UrlDecode();

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


        public async Task<UpdateUserMailVerifyResponse> UpdateUserMailVerify(UpdateUserMailVerifyRequest request)
        {
            User user = await _userManager.FindByIdAsync(request.UserId.ToString());
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

            if (string.IsNullOrWhiteSpace(request.EmailConfirmToken))
            {
                throw new BadRequestException("Doğrulama bağlantısı geçersiz.");
            }

            string token = request.EmailConfirmToken.UrlDecode();
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                user.EmailVerificationSentAt = null;

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


        public async Task<UpdateUserEmailResponse> UpdateUserEmailAsync(UpdateUserEmailRequest request)
        {
            var newEmail = request.NewEmail.Trim().ToLowerInvariant();

            User user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(request.ChangeEmailToken))
            {
                throw new BadRequestException("Doğrulama bağlantısı geçersiz.");
            }
            string token = request.ChangeEmailToken.UrlDecode();
            IdentityResult result = await _userManager.ChangeEmailAsync(user, newEmail, token);

            if (result.Succeeded)
            {
                var usernameResult = await _userManager.SetUserNameAsync(user, newEmail);
                if (!usernameResult.Succeeded)
                {
                    throw new ChangeEmailFailedException("Kullanıcı adı email ile eşitlenemedi.");
                }
                   
                await _userManager.UpdateSecurityStampAsync(user);
                await _userManager.UpdateAsync(user);

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

        private static string GetIdentityErrors(IdentityResult result) => string.Join(", ", result.Errors.Select(error => error.Description));



    }
}
