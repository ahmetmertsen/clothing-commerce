using FluentValidation;
using MediatR;
using shipment_service.Application.Repositories;
using shipment_service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Features.Shipment.Commands.Create
{
    public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, CreateShipmentCommandResponse>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IValidator<CreateShipmentCommand> _validator;

        public CreateShipmentCommandHandler(IShipmentRepository shipmentRepository, IValidator<CreateShipmentCommand> validator)
        {
            _shipmentRepository = shipmentRepository;
            _validator = validator;
        }

        public async Task<CreateShipmentCommandResponse> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var shipment = new Domain.Entities.Shipment(request.OrderId, request.CustomerId, request.CarrierCompany);

            await _shipmentRepository.AddAsync(shipment);
            await _shipmentRepository.SaveChangesAsync();

            return new CreateShipmentCommandResponse
            {
                Succeeded = true,
                Message = "Kargo kaydı başarıyla oluşturuldu.",
                ShipmentId = shipment.Id,
                TrackingNumber = shipment.TrackingNumber
            };
        }
    }
}
