using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Queries.GetRolesToUser
{
    public class GetRolesToUserQueryValidator : AbstractValidator<GetRolesToUserQuery>
    {
        public GetRolesToUserQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı bilgisi boş olamaz");
        }
    }
}
