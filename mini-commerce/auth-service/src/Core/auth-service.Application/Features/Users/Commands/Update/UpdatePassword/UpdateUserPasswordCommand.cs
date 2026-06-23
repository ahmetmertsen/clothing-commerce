using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Commands.Update.UpdatePassword
{
    public class UpdateUserPasswordCommand : IRequest<UpdateUserPasswordCommandResponse>
    {
        public Guid UserId { get; set; }
        public string ResetToken { get; set; } = null!;
        public string newPassword { get; set; } = null!;
        public string newPasswordConfirmed { get; set; } = null!;
    }
}
