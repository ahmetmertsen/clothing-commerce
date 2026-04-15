using FluentValidation;
using MediatR;
using product_service.Application.Repositories;
using product_service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Application.Features.Product.Commands.AddVariant
{
    public class AddProductVariantCommandHandler : IRequestHandler<AddProductVariantCommand, AddProductVariantCommandResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<AddProductVariantCommand> _validator;

        public AddProductVariantCommandHandler(IProductRepository productRepository, IValidator<AddProductVariantCommand> validator)
        {
            _productRepository = productRepository;
            _validator = validator;
        }

        public async Task<AddProductVariantCommandResponse> Handle(AddProductVariantCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var product = await _productRepository.GetByIdWithVariantsAsync(request.ProductId);
            if (product == null)
            {
                throw new Exception("Ürün bulunamadı.");
            }

            bool variantExists = product.Variants.Any(v => v.Size == request.Size && v.Color == request.Color);
            if (variantExists == true)
            {
                throw new Exception("Aynı beden ve renk kombinasyonuna sahip varyant zaten mevcut.");
            }

            var variant = new ProductVariant(request.Size, request.Color, request.Sku, request.Price, request.StockQuantity);
            variant.ProductId = product.Id;

            await _productRepository.AddVariantAsync(variant);
            await _productRepository.SaveChangesAsync();

            return new AddProductVariantCommandResponse
            {
                Succeeded = true,
                Message = "Ürün varyantı başarıyla eklendi.",
                ProductId = product.Id,
                VariantId = variant.Id
            };
        }
    }
}
