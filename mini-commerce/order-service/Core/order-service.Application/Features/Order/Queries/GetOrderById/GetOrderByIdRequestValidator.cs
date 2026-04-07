using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application.Features.Order.Queries.GetOrderById
{
    public class GetOrderByIdRequestValidator : AbstractValidator<GetOrderByIdRequest>
    {
        public GetOrderByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Sipariş Id'si boş olamaz.");
        }
    }
}
