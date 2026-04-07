using MediatR;
using order_service.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application.Features.Order.Queries.GetOrdersByCustomerId
{
    public record GetOrdersByCustomerIdRequest : IRequest<List<OrderDto>>
    {
        public Guid CustomerId { get; set; }
    }
}
