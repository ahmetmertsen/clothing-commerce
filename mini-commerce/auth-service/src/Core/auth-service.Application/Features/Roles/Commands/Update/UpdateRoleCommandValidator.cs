using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Roles.Commands.Update
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Rol alanı boş olmamalı.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Rol adı boş olamaz.")
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Rol adı yalnızca boşluklardan oluşamaz.")
                .MaximumLength(100).WithMessage("Rol adı en fazla 100 karakter olabilir.");
        }
    }
}
