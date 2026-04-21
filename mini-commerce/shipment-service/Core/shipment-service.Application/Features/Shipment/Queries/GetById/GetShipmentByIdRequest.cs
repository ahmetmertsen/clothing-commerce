using MediatR;
using shipment_service.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Features.Shipment.Queries.GetById
{
    public class GetShipmentByIdRequest : IRequest<ShipmentDto>
    {
        public Guid Id { get; set; }
    }
}
