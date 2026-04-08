using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cart_service.Application.Features.Cart.Commands.ChangeCartItemQuantity
{
    public class ChangeCartItemQuantityCommandResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = null!;
        public Guid CartId { get; set; }
    }
}
