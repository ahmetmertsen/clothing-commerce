using FluentValidation;
using MediatR;
using order_service.Application.Repositories;
using order_service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application.Features.Order.Commands.Create
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderCommandResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IValidator<CreateOrderCommand> _validator;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IValidator<CreateOrderCommand> validator)
        {
            _orderRepository = orderRepository;
            _validator = validator;
        }

        public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderItems = request.OrderItems.Select(x =>
                new OrderItem(x.ProductId, x.ProductName, x.UnitPrice, x.Quantity)).ToList();

            var order = new Domain.Entities.Order(request.CustomerId, orderItems);

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return new CreateOrderCommandResponse
            {
                Succeeded = true,
                Message = "Sipariş başarıyla oluşturuldu.",
                OrderId = order.Id,
                OrderNumber = order.OrderNumber
            };

        }
    }
}
