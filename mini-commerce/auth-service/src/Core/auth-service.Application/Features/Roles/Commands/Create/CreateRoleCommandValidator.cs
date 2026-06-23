using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Roles.Commands.Create
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Rol adı boş olamaz.")
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Rol adı yalnızca boşluklardan oluşamaz.")
                .MaximumLength(100).WithMessage("Rol adı en fazla 100 karakter olabilir.");
        }
    }
}
