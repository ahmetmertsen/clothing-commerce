using product_service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Application.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task AddAsync(Product product);
        void Update(Product product);
        Task SaveChangesAsync();

        Task<Product?> GetByIdWithVariantsAsync(Guid id);
        Task<ProductVariant?> GetVariantByIdAsync(Guid variantId);
        Task AddVariantAsync(ProductVariant variant);
    }
}
