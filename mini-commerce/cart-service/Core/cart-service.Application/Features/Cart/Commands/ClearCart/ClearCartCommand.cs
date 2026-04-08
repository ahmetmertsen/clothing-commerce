using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cart_service.Application.Features.Cart.Commands.ClearCart
{
    public class ClearCartCommand : IRequest<ClearCartCommandResponse>
    {
        public Guid CustomerId { get; set; }
    }
}
