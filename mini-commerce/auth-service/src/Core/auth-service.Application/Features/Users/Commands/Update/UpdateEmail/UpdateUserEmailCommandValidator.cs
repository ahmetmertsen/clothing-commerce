using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Commands.Update.UpdateEmail
{
    public class UpdateUserEmailCommandValidator : AbstractValidator<UpdateUserEmailCommand>
    {
        public UpdateUserEmailCommandValidator()
        {
            RuleFor(x => x.ChangeEmailToken)
                .NotEmpty().WithMessage("Email değiştirme token bilgisi boş olamaz.");

            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("Yeni email boş olamaz.")
                .EmailAddress().WithMessage("Yeni email formatı geçersiz.");
        }
    }
}
