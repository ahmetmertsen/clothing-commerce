using MediatR;
using payment_service.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Features.Payment.Queries.GetPaymentById
{
    public class GetPaymentByIdRequest : IRequest<PaymentDto>
    {
        public Guid Id { get; set; }
    }
}
