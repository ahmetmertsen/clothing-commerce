using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application.Features.Order.Queries.GetOrderByOrderNumber
{
    public class GetOrderByOrderNumberRequestValidator : AbstractValidator<GetOrderByOrderNumberRequest>
    {
        public GetOrderByOrderNumberRequestValidator()
        {
            RuleFor(x => x.OrderNumber)
                .NotEmpty().WithMessage("Sipariş numarası boş olamaz");
        }
    }
}
