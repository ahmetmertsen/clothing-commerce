using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application.Features.Order.Commands.Create
{
    public class CreateOrderCommandResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = null!;
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
    }
}
