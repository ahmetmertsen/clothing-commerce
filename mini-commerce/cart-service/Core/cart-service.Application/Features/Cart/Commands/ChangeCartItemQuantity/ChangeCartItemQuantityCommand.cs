using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cart_service.Application.Features.Cart.Commands.ChangeCartItemQuantity
{
    public class ChangeCartItemQuantityCommand : IRequest<ChangeCartItemQuantityCommandResponse>
    {
        public Guid CustomerId { get; set; }
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
