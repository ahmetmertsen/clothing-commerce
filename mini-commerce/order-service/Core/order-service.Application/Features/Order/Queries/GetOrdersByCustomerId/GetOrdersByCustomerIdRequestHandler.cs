using FluentValidation;
using MediatR;
using order_service.Application.DTOs;
using order_service.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application.Features.Order.Queries.GetOrdersByCustomerId
{
    public class GetOrdersByCustomerIdRequestHandler : IRequestHandler<GetOrdersByCustomerIdRequest, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IValidator<GetOrdersByCustomerIdRequest> _validator;

        public GetOrdersByCustomerIdRequestHandler(IOrderRepository orderRepository, IValidator<GetOrdersByCustomerIdRequest> validator)
        {
            _orderRepository = orderRepository;
            _validator = validator;
        }

        public async Task<List<OrderDto>> Handle(GetOrdersByCustomerIdRequest request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orders = await _orderRepository.GetByCustomerIdWithItemsAsync(request.CustomerId);

            return orders.Select(order => new OrderDto
            {
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedDate = order.CreatedDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    LineTotal = oi.LineTotal
                }).ToList()
            }).ToList();
        }
    }
}
