using cart_service.Application.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cart_service.Application.Features.Cart.Commands.ClearCart
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, ClearCartCommandResponse>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IValidator<ClearCartCommand> _validator;

        public ClearCartCommandHandler(ICartRepository cartRepository, IValidator<ClearCartCommand> validator)
        {
            _cartRepository = cartRepository;
            _validator = validator;
        }

        public async Task<ClearCartCommandResponse> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var cart = await _cartRepository.GetByCustomerIdWithItemsAsync(request.CustomerId);
            if (cart == null)
            {
                throw new Exception("Sepet bulunamadı");
            }

            cart.CartItems.Clear();
            cart.TotalAmount = 0;
            cart.UpdatedDate = DateTime.UtcNow;

            await _cartRepository.SaveChangesAsync();

            return new ClearCartCommandResponse
            {
                Succeeded = true,
                Message = "Sepet başarıyla temizlendi.",
                CartId = cart.Id
            };
        }
    }
}
