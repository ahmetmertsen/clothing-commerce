using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Features.Shipment.Commands.Create
{
    public class CreateShipmentCommandResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = null!;
        public Guid ShipmentId { get; set; }
        public string TrackingNumber { get; set; } = null!;
    }
}
