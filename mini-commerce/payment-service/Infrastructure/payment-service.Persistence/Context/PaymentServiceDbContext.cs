using Microsoft.EntityFrameworkCore;
using payment_service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Persistence.Context
{
    public class PaymentServiceDbContext : DbContext
    {
        public PaymentServiceDbContext(DbContextOptions<PaymentServiceDbContext> options) : base(options) { }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(p => p.Amount)
                    .HasPrecision(18, 2);

                entity.Property(p => p.Status)
                    .HasConversion<string>();

                entity.HasIndex(p => p.TransactionId)
                    .IsUnique();

                entity.HasIndex(p => p.CustomerId);
                entity.HasIndex(p => p.CartId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
