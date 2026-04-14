using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        private Product() { }

        public Product(string name, string description, string brand, string categoryName)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Brand = brand;
            CategoryName = categoryName;
            CreatedDate = DateTime.UtcNow;
        }
    }
}
