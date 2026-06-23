using auth_service.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.MailVerify
{
    public class MailVerifyCommand : IRequest<MailVerifyCommandResponse>, ICurrentUserRequest, IAllowUnverifiedEmail
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
