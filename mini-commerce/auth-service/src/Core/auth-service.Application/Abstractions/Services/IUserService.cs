using auth_service.Application.Dtos.User;
using auth_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<RegisterUserResponseDto> RegisterAsync(RegisterUserRequestDto request, CancellationToken cancellationToken);
        Task<UpdateUserPasswordResponse> UpdatePasswordAsync(UpdateUserPasswordRequest request, CancellationToken cancellationToken);
        Task<UpdateUserMailVerifyResponse> UpdateUserMailVerify(UpdateUserMailVerifyRequest request);
        Task<UpdateUserEmailResponse> UpdateUserEmailAsync(UpdateUserEmailRequest request);
        Task<(List<AdminUserDto> Items, int TotalCount)> GetPagedUsersAsync(int page, int size, string? search, UserStatus? status, bool? emailConfirmed, CancellationToken cancellationToken);
        Task<UserDto> GetUserById(Guid userId);
        Task AssignRoleToUserAsync(Guid actorUserId, Guid targetUserId, string[] roles, CancellationToken cancellationToken);
        Task<string[]> GetRolesToUserAsync(Guid userId);
    }
}
