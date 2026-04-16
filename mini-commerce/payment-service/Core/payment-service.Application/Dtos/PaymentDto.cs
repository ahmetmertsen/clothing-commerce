using payment_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Dtos
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CartId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? FailureReason { get; set; }
    }
}
