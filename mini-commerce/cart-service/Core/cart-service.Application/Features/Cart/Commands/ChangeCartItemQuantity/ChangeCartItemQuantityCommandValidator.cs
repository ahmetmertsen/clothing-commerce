using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cart_service.Application.Features.Cart.Commands.ChangeCartItemQuantity
{
    public class ChangeCartItemQuantityCommandValidator : AbstractValidator<ChangeCartItemQuantityCommand>
    {
        public ChangeCartItemQuantityCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Müşteri bilgisi boş olamaz.");

            RuleFor(x => x.CartItemId)
                .NotEmpty()
                .WithMessage("Sepet ürünü bilgisi boş olamaz.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Ürün adedi sıfırdan büyük olmalıdır.");
        }
    }
}
