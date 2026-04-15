using FluentValidation;
using MediatR;
using product_service.Application.Dtos;
using product_service.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Application.Features.Product.Queries.GetByVariantId
{
    public class GetProductByVariantIdRequestHandler : IRequestHandler<GetProductByVariantIdRequest, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<GetProductByVariantIdRequest> _validator;

        public GetProductByVariantIdRequestHandler(IProductRepository productRepository, IValidator<GetProductByVariantIdRequest> validator)
        {
            _productRepository = productRepository;
            _validator = validator;
        }

        public async Task<ProductDto> Handle(GetProductByVariantIdRequest request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var variant = await _productRepository.GetVariantByIdAsync(request.VariantId);
            if (variant == null)
            {
                throw new Exception("Ürün varyantı bulunamadı.");
            }

            var product = await _productRepository.GetByIdWithVariantsAsync(variant.ProductId);
            if (product == null)
            {
                throw new Exception("Ürün bulunamadı.");
            }

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                CategoryName = product.CategoryName,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
                Variants = product.Variants.Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    ProductId = v.ProductId,
                    Size = v.Size,
                    Color = v.Color,
                    Sku = v.Sku,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity
                }).ToList()
            };
        }
    }
}
