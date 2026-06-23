using FluentValidation;

namespace auth_service.Application.Features.Auth.ChangeEmail
{
    public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
    {
        public ChangeEmailCommandValidator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("Yeni Email bilgisi boş olamaz.")
                .EmailAddress().WithMessage("Yeni Email formatı geçersiz.");
        }
    }
}
