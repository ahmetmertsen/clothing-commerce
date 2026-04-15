using FluentValidation;
using MediatR;
using product_service.Application.Dtos;
using product_service.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Application.Features.Product.Queries.GetById
{
    public class GetProductByIdRequestHandler : IRequestHandler<GetProductByIdRequest, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<GetProductByIdRequest> _validator;

        public GetProductByIdRequestHandler(IProductRepository productRepository, IValidator<GetProductByIdRequest> validator)
        {
            _productRepository = productRepository;
            _validator = validator;
        }

        public async Task<ProductDto> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var product = await _productRepository.GetByIdWithVariantsAsync(request.Id);
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
                Variants = product.Variants.Select(variant => new ProductVariantDto
                {
                    Id = variant.Id,
                    ProductId = variant.ProductId,
                    Size = variant.Size,
                    Color = variant.Color,
                    Sku = variant.Sku,
                    Price = variant.Price,
                    StockQuantity = variant.StockQuantity
                }).ToList()
            };
        }
    }
}
