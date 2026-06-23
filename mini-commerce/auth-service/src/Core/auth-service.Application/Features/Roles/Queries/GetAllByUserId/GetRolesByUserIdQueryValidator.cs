using auth_service.Application.Features.Roles.Commands.Update;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Roles.Queries.GetAllByUserId
{
    public class GetRolesByUserIdQueryValidator : AbstractValidator<GetRolesByUserIdQuery>
    {
        public GetRolesByUserIdQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Rol alanı boş olmamalı.");
        }
    }
}
