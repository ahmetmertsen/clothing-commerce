using auth_service.Application.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Roles.Commands.Update
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, UpdateRoleCommandResponse>
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;

        public UpdateRoleCommandHandler(IRoleService roleService, ILogger<UpdateRoleCommandHandler> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        public async Task<UpdateRoleCommandResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            await _roleService.UpdateRole(request.Id, request.Name, cancellationToken);
            _logger.LogInformation("Role updated. RoleId: {RoleId}, RoleName: {RoleName}", request.Id, request.Name.Trim());
            return new UpdateRoleCommandResponse { Succeeded = true, Message = "Rol başarıyla güncellendi." };
        }
    }
}
