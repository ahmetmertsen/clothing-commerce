using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Features.Payment.Commands.ProcessPayment
{
    public class ProcessPaymentCommandResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = null!;
        public Guid PaymentId { get; set; }
        public string TransactionId { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
