using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cart_service.Application.Features.Cart.Commands.RemoveItemFromCart
{
    public class RemoveItemFromCartCommandValidator : AbstractValidator<RemoveItemFromCartCommand>
    {
        public RemoveItemFromCartCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Müşteri bilgisi boş olamaz.");

            RuleFor(x => x.CartItemId)
                .NotEmpty()
                .WithMessage("Sepetten silinecek ürün bilgisi boş olamaz.");
        }
    }
}
