using order_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application.DTOs
{
    public class OrderDto
    {
        public string OrderNumber { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
 