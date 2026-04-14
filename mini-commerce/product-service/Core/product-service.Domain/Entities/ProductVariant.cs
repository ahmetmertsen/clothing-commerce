using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Domain.Entities
{
    public class ProductVariant
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public string Size { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public Product? Product { get; set; }

        private ProductVariant() { }

        public ProductVariant(string size, string color, string sku, decimal price, int stockQuantity)
        {
            Id = Guid.NewGuid();
            Size = size;
            Color = color;
            Sku = sku;
            Price = price;
            StockQuantity = stockQuantity;
        }
    }
}
