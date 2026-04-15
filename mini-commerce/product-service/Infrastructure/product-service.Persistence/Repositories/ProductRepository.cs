using Microsoft.EntityFrameworkCore;
using product_service.Application.Repositories;
using product_service.Domain.Entities;
using product_service.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductServiceDbContext _context;

        public ProductRepository(ProductServiceDbContext context)
        {
            _context = context;
        }


        public async Task<List<Product>> GetAllAsync() => await _context.Products
            .Include(p => p.Variants)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();

        public async Task<Product?> GetByIdAsync(Guid id) => await _context.Products.FindAsync(id);

        public async Task AddAsync(Product product) => await _context.Products.AddAsync(product);

        public void Update(Product product) => _context.Products.Update(product);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<Product?> GetByIdWithVariantsAsync(Guid id) => await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<ProductVariant?> GetVariantByIdAsync(Guid variantId) => await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.Id == variantId);

        public async Task AddVariantAsync(ProductVariant variant) => await _context.ProductVariants.AddAsync(variant);
    }
}
