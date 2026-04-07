using MediatR;
using order_service.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application.Features.Order.Queries.GetOrderByOrderNumber
{
    public class GetOrderByOrderNumberRequest : IRequest<OrderDto>
    {
        public string OrderNumber { get; set; } = null!;
    }
}
