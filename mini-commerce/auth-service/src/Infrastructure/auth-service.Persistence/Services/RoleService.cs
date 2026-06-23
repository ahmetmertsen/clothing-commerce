using auth_service.Application.Abstractions.Services;
using auth_service.Application.Common.Consts;
using auth_service.Application.Dtos.Role;
using auth_service.Application.Exceptions;
using auth_service.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace auth_service.Persistence.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public RoleService(RoleManager<Role> roleManager, UserManager<User> userManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<List<RoleDto>> GetAllRoles(CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.AsNoTracking().OrderBy(role => role.Name).ToListAsync(cancellationToken);
            return _mapper.Map<List<RoleDto>>(roles);
        }

        public async Task<List<RoleDto>> GetRolesByEmail(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }

            var userRoleNames = await _userManager.GetRolesAsync(user);
            if (userRoleNames.Count == 0)
            {
                return new List<RoleDto>();
            }

            var roles = await _roleManager.Roles.AsNoTracking().Where(role => role.Name != null && userRoleNames.Contains(role.Name)).OrderBy(role => role.Name).ToListAsync(cancellationToken);
            return _mapper.Map<List<RoleDto>>(roles);
        }

        public async Task<RoleDto> GetRoleById(Guid id, CancellationToken cancellationToken)
        {
            var role = await _roleManager.Roles.AsNoTracking().FirstOrDefaultAsync(role => role.Id == id, cancellationToken);
            if (role == null)
            {
                throw new NotFoundException("Rol bulunamadı.");
            }

            return _mapper.Map<RoleDto>(role);
        }

        public async Task CreateRole(string name, CancellationToken cancellationToken)
        {
            var roleName = name.Trim();
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                throw new BadRequestException("Bu rol adı zaten kullanılıyor.");
            }

            var result = await _roleManager.CreateAsync(new Role { Name = roleName });
            EnsureSucceeded(result, "Rol oluşturulamadı");
        }

        public async Task DeleteRole(Guid id, CancellationToken cancellationToken)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (role == null)
            {
                throw new NotFoundException("Rol bulunamadı.");
            }

            EnsureRoleCanBeChanged(role);

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Count > 0)
            {
                throw new BadRequestException("Kullanıcılara atanmış bir rol silinemez. Önce kullanıcı rol atamalarını kaldırın.");
            }

            var result = await _roleManager.DeleteAsync(role);
            EnsureSucceeded(result, "Rol silinemedi");
        }

        

        

        

        public async Task UpdateRole(Guid id, string name, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                throw new NotFoundException("Rol bulunamadı.");
            }

            EnsureRoleCanBeChanged(role);

            var roleName = name.Trim();
            var normalizedRoleName = _roleManager.NormalizeKey(roleName);
            var duplicateExists = await _roleManager.Roles.AsNoTracking().AnyAsync(existingRole => existingRole.Id != id && existingRole.NormalizedName == normalizedRoleName, cancellationToken);
            if (duplicateExists)
            {
                throw new BadRequestException("Bu rol adı zaten kullanılıyor.");
            }

            role.Name = roleName;
            var result = await _roleManager.UpdateAsync(role);
            EnsureSucceeded(result, "Rol güncellenemedi");
        }

        private static void EnsureRoleCanBeChanged(Role role)
        {
            if (role.Name != null && (role.Name.Equals(RoleConstants.Admin, StringComparison.OrdinalIgnoreCase) || role.Name.Equals(RoleConstants.Moderator, StringComparison.OrdinalIgnoreCase) || role.Name.Equals(RoleConstants.User, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BadRequestException("Sistem rolleri güncellenemez veya silinemez.");
            }
        }

        private static void EnsureSucceeded(IdentityResult result, string message)
        {
            if (!result.Succeeded)
            {
                throw new BadRequestException($"{message}: {string.Join(", ", result.Errors.Select(error => error.Description))}");
            }
        }
    }
}
