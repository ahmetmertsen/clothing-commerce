using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cart_service.Application.Features.Cart.Commands.RemoveItemFromCart
{
    public class RemoveItemFromCartCommand : IRequest<RemoveItemFromCartCommandResponse>
    {
        public Guid CustomerId { get; set; }
        public Guid CartItemId { get; set; }
    }
}
