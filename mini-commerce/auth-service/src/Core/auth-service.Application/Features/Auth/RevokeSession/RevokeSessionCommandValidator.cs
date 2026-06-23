using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.RevokeSession
{
    public class RevokeSessionCommandValidator : AbstractValidator<RevokeSessionCommand>
    {
        public RevokeSessionCommandValidator()
        {
            RuleFor(command => command.SessionId)
                .NotEmpty().WithMessage("Oturum kimliği boş olamaz.");
        }
    }
}
