using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Features.Payment.Queries.GetPaymentById
{
    public class GetPaymentByIdRequestValidator : AbstractValidator<GetPaymentByIdRequest>
    {
        public GetPaymentByIdRequestValidator() 
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Ödeme bilgisi boş olamaz.");
        }
    }
}
