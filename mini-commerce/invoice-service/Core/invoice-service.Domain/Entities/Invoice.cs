using invoice_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice_service.Domain.Entities
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        private Invoice() { }

        public Invoice(Guid orderId, Guid customerId, decimal totalAmount)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException("Geçerli bir sipariş bilgisi girilmelidir.");

            if (customerId == Guid.Empty)
                throw new ArgumentException("Geçerli bir müşteri bilgisi girilmelidir.");

            if (totalAmount <= 0)
                throw new ArgumentException("Fatura tutarı sıfırdan büyük olmalıdır.");

            Id = Guid.NewGuid();
            OrderId = orderId;
            CustomerId = customerId;
            TotalAmount = totalAmount;
            InvoiceNumber = GenerateInvoiceNumber();
            Status = InvoiceStatus.Created;
            CreatedDate = DateTime.UtcNow;
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

    }
}
