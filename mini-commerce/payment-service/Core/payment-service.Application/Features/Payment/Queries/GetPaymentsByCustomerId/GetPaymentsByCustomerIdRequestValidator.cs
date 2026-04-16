using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Features.Payment.Queries.GetPaymentsByCustomerId
{
    public class GetPaymentsByCustomerIdRequestValidator : AbstractValidator<GetPaymentsByCustomerIdRequest>
    {
        public GetPaymentsByCustomerIdRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Müşteri bilgisi boş olamaz.");
        }
    }
}
