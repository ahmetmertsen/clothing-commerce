using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Auth.RefreshTokenLogin
{
    public class RefreshTokenLoginCommandValidator : AbstractValidator<RefreshTokenLoginCommand>
    {
        public RefreshTokenLoginCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token boş olamaz.");
        }
    }
}
