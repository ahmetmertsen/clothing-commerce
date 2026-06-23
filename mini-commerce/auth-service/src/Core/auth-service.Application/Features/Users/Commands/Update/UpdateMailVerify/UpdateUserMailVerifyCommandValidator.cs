using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Commands.Update.UpdateMailVerify
{
    public class UpdateUserMailVerifyCommandValidator : AbstractValidator<UpdateUserMailVerifyCommand>
    {
        public UpdateUserMailVerifyCommandValidator()
        {
            RuleFor(x => x.EmailConfirmToken)
                .NotEmpty().WithMessage("Email doğrulama token bilgisi boş olamaz.");
        }
    }
}
