using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Features.Payment.Commands.ProcessPayment
{
    public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
    {
        public ProcessPaymentCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Müşteri bilgisi boş olamaz.");

            RuleFor(x => x.CartId)
                .NotEmpty().WithMessage("Sepet bilgisi boş olamaz.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Ödeme tutarı sıfırdan büyük olmalıdır.");

            RuleFor(x => x.CardHolderName)
                .NotEmpty().WithMessage("Kart sahibi adı boş olamaz.");

            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Kart numarası boş olamaz.");

            RuleFor(x => x.ExpireMonth)
                .NotEmpty().WithMessage("Son kullanma ayı boş olamaz.");

            RuleFor(x => x.ExpireYear)
                .NotEmpty().WithMessage("Son kullanma yılı boş olamaz.");

            RuleFor(x => x.Cvv)
                .NotEmpty().WithMessage("CVV bilgisi boş olamaz.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Ödeme yöntemi boş olamaz.");
        }
    }
}
