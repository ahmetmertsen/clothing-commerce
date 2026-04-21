using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Features.Shipment.Queries.GetById
{
    public class GetShipmentByIdRequestValidator : AbstractValidator<GetShipmentByIdRequest>
    {
        public GetShipmentByIdRequestValidator() 
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Kargo bilgisi boş olamaz.");
        }
    }
}
