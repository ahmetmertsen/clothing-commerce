using MediatR;
using payment_service.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Features.Payment.Queries.GetPaymentsByCustomerId
{
    public class GetPaymentsByCustomerIdRequest : IRequest<List<PaymentDto>>
    {
        public Guid CustomerId { get; set; }
    }
}
