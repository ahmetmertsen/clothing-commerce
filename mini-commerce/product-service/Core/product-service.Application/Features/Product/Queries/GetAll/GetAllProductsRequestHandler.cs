using MediatR;
using product_service.Application.Dtos;
using product_service.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Application.Features.Product.Queries.GetAll
{
    public class GetAllProductsRequestHandler : IRequestHandler<GetAllProductsRequest, List<ProductDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductsRequestHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductDto>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(product => new ProductDto
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
            }).ToList();
        }
    }
}
