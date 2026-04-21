using FluentValidation;
using MediatR;
using shipment_service.Application.Dtos;
using shipment_service.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Features.Shipment.Queries.GetById
{
    public class GetShipmentByIdRequestHandler : IRequestHandler<GetShipmentByIdRequest, ShipmentDto>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IValidator<GetShipmentByIdRequest> _validator;

        public GetShipmentByIdRequestHandler(IShipmentRepository shipmentRepository, IValidator<GetShipmentByIdRequest> validator)
        {
            _shipmentRepository = shipmentRepository;
            _validator = validator;
        }

        public async Task<ShipmentDto> Handle(GetShipmentByIdRequest request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var shipment = await _shipmentRepository.GetByIdAsync(request.Id);
            if (shipment == null)
            {
                throw new Exception("Kargo kaydı bulanamadı.");
            }

            return new ShipmentDto
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                CustomerId = shipment.CustomerId,
                TrackingNumber = shipment.TrackingNumber,
                CarrierCompany = shipment.CarrierCompany,
                Status = shipment.Status,
                CreatedDate = shipment.CreatedDate
            };
        }
    }
}
