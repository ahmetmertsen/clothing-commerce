using invoice_service.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice_service.Persistence.Context
{
    public class InvoiceServiceDbContext : DbContext
    {
        public InvoiceServiceDbContext(DbContextOptions<InvoiceServiceDbContext> options) : base(options) { }

        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.InvoiceNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.TotalAmount)
                    .HasPrecision(18, 2);

                entity.Property(x => x.Status)
                    .HasConversion<string>();

                entity.HasIndex(x => x.InvoiceNumber)
                    .IsUnique();

                entity.HasIndex(x => x.OrderId);
                entity.HasIndex(x => x.CustomerId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
