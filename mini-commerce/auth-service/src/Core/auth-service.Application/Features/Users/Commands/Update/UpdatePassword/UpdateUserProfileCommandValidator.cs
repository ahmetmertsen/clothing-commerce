using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Commands.Update.UpdatePassword
{
    public class UpdateUserPasswordCommandValidator : AbstractValidator<UpdateUserPasswordCommand>
    {
        public UpdateUserPasswordCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User Id boş olmamalı.");

            RuleFor(x => x.ResetToken)
                .NotEmpty().WithMessage("Şifre sıfırlama token bilgisi boş olamaz.");

            RuleFor(x => x.newPassword)
                .NotEmpty().WithMessage("Yeni şifre boş olamaz.")
                .MinimumLength(6).WithMessage("Yeni şifre en az 6 karakter olmalıdır.");

            RuleFor(x => x.newPasswordConfirmed)
                .NotEmpty().WithMessage("Yeni şifre tekrarı boş olamaz.")
                .MinimumLength(6).WithMessage("Yeni şifre tekrarı en az 6 karakter olmalıdır.")
                .Equal(x => x.newPassword).WithMessage("Şifreler uyuşmuyor.");
        }
    }
}
