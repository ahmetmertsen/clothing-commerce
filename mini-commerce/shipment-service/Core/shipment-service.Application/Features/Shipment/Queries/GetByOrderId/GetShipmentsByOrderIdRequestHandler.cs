using FluentValidation;
using MediatR;
using shipment_service.Application.Dtos;
using shipment_service.Application.Repositories;
using shipment_service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Features.Shipment.Queries.GetByOrderId
{
    public class GetShipmentsByOrderIdRequestHandler : IRequestHandler<GetShipmentsByOrderIdRequest, List<ShipmentDto>>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IValidator<GetShipmentsByOrderIdRequest> _validator;

        public GetShipmentsByOrderIdRequestHandler(IShipmentRepository shipmentRepository, IValidator<GetShipmentsByOrderIdRequest> validator)
        {
            _shipmentRepository = shipmentRepository;
            _validator = validator;
        }

        public async Task<List<ShipmentDto>> Handle(GetShipmentsByOrderIdRequest request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var shipments = await _shipmentRepository.GetByOrderIdAsync(request.OrderId);

            return shipments.Select(shipment => new ShipmentDto
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                CustomerId = shipment.CustomerId,
                TrackingNumber = shipment.TrackingNumber,
                CarrierCompany = shipment.CarrierCompany,
                Status = shipment.Status,
                CreatedDate = shipment.CreatedDate
            }).ToList();
        }
    }
}
