using auth_service.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Commands.AssignRoleToUser
{
    public class AssignRoleToUserCommand : IRequest<AssignRoleToUserCommandResponse>, ICurrentUserRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        [JsonPropertyName("userId")]
        public Guid TargetUserId { get; set; }

        public string[] Roles { get; set; } = null!;
    }
}
