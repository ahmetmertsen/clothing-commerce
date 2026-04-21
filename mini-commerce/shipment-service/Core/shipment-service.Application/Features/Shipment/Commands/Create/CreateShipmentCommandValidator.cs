using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Features.Shipment.Commands.Create
{
    public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
    {
        public CreateShipmentCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Sipariş bilgisi boş olamaz");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Müşteri bilgisi boş olamaz");

            RuleFor(x => x.CarrierCompany)
                .NotEmpty().WithMessage("Kargo firması boş olamaz.")
                .MaximumLength(100).WithMessage("Kargo firması en fazla 100 karakter olabilir.");
        }
    }
}
