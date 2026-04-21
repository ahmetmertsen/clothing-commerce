using MediatR;
using shipment_service.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Features.Shipment.Queries.GetByOrderId
{
    public class GetShipmentsByOrderIdRequest : IRequest<List<ShipmentDto>>
    {
        public Guid OrderId { get; set; }
    }
}
