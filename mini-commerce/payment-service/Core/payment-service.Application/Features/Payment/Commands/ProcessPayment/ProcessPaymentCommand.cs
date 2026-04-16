using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Features.Payment.Commands.ProcessPayment
{
    public class ProcessPaymentCommand : IRequest<ProcessPaymentCommandResponse>
    {
        public Guid CustomerId { get; set; }
        public Guid CartId { get; set; }
        public decimal Amount { get; set; }

        public string CardHolderName { get; set; } = null!;
        public string CardNumber { get; set; } = null!;
        public string ExpireMonth { get; set; } = null!;
        public string ExpireYear { get; set; } = null!;
        public string Cvv { get; set; } = null!;

        public string PaymentMethod { get; set; } = "CreditCard";
        public bool SimulateSuccess { get; set; }
    }
}
