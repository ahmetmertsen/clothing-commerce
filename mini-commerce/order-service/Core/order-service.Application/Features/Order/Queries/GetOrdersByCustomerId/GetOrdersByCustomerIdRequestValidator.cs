using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application.Features.Order.Queries.GetOrdersByCustomerId
{
    public class GetOrdersByCustomerIdRequestValidator : AbstractValidator<GetOrdersByCustomerIdRequest>
    {
        public GetOrdersByCustomerIdRequestValidator() 
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Müşteri numarası boş olamaz.");
        }
    }
}
