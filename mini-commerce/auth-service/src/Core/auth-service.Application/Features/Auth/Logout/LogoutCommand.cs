using auth_service.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.Logout
{
    public class LogoutCommand : IRequest<LogoutCommandResponse>, ICurrentUserRequest, ICurrentSessionRequest, IAllowUnverifiedEmail
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        [JsonIgnore]
        public Guid CurrentSessionId { get; set; }
    }
}
