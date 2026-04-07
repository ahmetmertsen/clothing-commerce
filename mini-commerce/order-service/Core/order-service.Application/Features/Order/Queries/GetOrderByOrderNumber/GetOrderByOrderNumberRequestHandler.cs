using FluentValidation;
using MediatR;
using order_service.Application.DTOs;
using order_service.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application.Features.Order.Queries.GetOrderByOrderNumber
{
    public class GetOrderByOrderNumberRequestHandler : IRequestHandler<GetOrderByOrderNumberRequest, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IValidator<GetOrderByOrderNumberRequest> _validator;

        public GetOrderByOrderNumberRequestHandler(IOrderRepository orderRepository, IValidator<GetOrderByOrderNumberRequest> validator)
        {
            _orderRepository = orderRepository;
            _validator = validator;
        }

        public async Task<OrderDto> Handle(GetOrderByOrderNumberRequest request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var order = await _orderRepository.GetByOrderNumberWithItemsAsync(request.OrderNumber);
            if (order == null)
            {
                throw new Exception("Sipariş bulunamadı.");
            }

            OrderDto orderDto = new()
            {
                CustomerId = order.CustomerId,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    LineTotal = oi.LineTotal
                }).ToList(),
                TotalAmount = order.TotalAmount,
                CreatedDate = order.CreatedDate
            };

            return orderDto;
        }
    }
}
