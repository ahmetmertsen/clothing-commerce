using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Features.Shipment.Commands.Create
{
    public class CreateShipmentCommand : IRequest<CreateShipmentCommandResponse>
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string CarrierCompany { get; set; } = null!;
    }
}
