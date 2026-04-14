using Microsoft.EntityFrameworkCore;
using product_service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Persistence.Context
{
    public class ProductServiceDbContext : DbContext
    {
        public ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariant>(entity =>
            {

                entity.Property(v => v.Price)
                    .HasPrecision(18, 2);

                entity.HasIndex(v => v.Sku)
                    .IsUnique();

                entity.HasIndex(v => new { v.ProductId, v.Size, v.Color })
                    .IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
