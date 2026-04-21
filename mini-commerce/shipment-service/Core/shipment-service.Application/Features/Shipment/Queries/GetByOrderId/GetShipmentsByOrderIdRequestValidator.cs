using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Features.Shipment.Queries.GetByOrderId
{
    public class GetShipmentsByOrderIdRequestValidator : AbstractValidator<GetShipmentsByOrderIdRequest>
    {
        public GetShipmentsByOrderIdRequestValidator() 
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Sipariş bilgisi boş olamaz.");
        }
    }
}
