using auth_service.Application.Dtos.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Abstractions.Services
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRoles(CancellationToken cancellationToken);
        Task<List<RoleDto>> GetRolesByEmail(string email, CancellationToken cancellationToken);
        Task<RoleDto> GetRoleById(Guid id, CancellationToken cancellationToken);
        Task CreateRole(string name, CancellationToken cancellationToken);
        Task DeleteRole(Guid id, CancellationToken cancellationToken);
        Task UpdateRole(Guid id, string name, CancellationToken cancellationToken);
    }
}
