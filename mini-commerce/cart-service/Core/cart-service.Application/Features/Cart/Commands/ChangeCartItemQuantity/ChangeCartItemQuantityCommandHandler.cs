using cart_service.Application.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cart_service.Application.Features.Cart.Commands.ChangeCartItemQuantity
{
    public class ChangeCartItemQuantityCommandHandler : IRequestHandler<ChangeCartItemQuantityCommand, ChangeCartItemQuantityCommandResponse>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IValidator<ChangeCartItemQuantityCommand> _validator;

        public ChangeCartItemQuantityCommandHandler(ICartRepository cartRepository, IValidator<ChangeCartItemQuantityCommand> validator)
        {
            _cartRepository = cartRepository;
            _validator = validator;
        }

        public async Task<ChangeCartItemQuantityCommandResponse> Handle(ChangeCartItemQuantityCommand request, CancellationToken cancellationToken)
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
                throw new Exception("Sepet bulunamadı.");
            }

            cartItem.Quantity = request.Quantity;
            cart.TotalAmount = cart.CartItems.Sum(x => x.LineTotal);
            cart.UpdatedDate = DateTime.UtcNow;

            await _cartRepository.SaveChangesAsync();

            return new ChangeCartItemQuantityCommandResponse
            {
                Succeeded = true,
                Message = "Sepetteki ürün adedi başarıyla güncellendi.",
                CartId = cart.Id
            };
        }
    }
}
