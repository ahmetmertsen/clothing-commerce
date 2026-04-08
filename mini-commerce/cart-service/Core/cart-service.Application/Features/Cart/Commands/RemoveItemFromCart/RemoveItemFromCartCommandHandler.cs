using cart_service.Application.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cart_service.Application.Features.Cart.Commands.RemoveItemFromCart
{
    public class RemoveItemFromCartCommandHandler : IRequestHandler<RemoveItemFromCartCommand, RemoveItemFromCartCommandResponse>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IValidator<RemoveItemFromCartCommand> _validator;

        public RemoveItemFromCartCommandHandler(ICartRepository cartRepository, IValidator<RemoveItemFromCartCommand> validator)
        {
            _cartRepository = cartRepository;
            _validator = validator;
        }

        public async Task<RemoveItemFromCartCommandResponse> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var cart = await _cartRepository.GetByCustomerIdWithItemsAsync(request.CustomerId);
            if (cart == null)
            {
                throw new Exception("Sepet bulunamadı.");
            }

            var cartItem = cart.CartItems.FirstOrDefault(x => x.Id == request.CartItemId);

            if (cartItem == null)
            {
                throw new Exception("Sepet ürün bulunamadı.");
            }

            cart.CartItems.Remove(cartItem);
            cart.TotalAmount = cart.CartItems.Sum(x => x.LineTotal);
            cart.UpdatedDate = DateTime.UtcNow;

            await _cartRepository.SaveChangesAsync();

            return new RemoveItemFromCartCommandResponse
            {
                Succeeded = true,
                Message = "Ürün sepetten başarıyla kaldırıldı.",
                CartId = cart.Id
            };
        }
    }
}
