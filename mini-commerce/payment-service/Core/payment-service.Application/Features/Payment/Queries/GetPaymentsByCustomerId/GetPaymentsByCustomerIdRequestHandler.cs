using FluentValidation;
using MediatR;
using payment_service.Application.Dtos;
using payment_service.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Features.Payment.Queries.GetPaymentsByCustomerId
{
    public class GetPaymentsByCustomerIdRequestHandler : IRequestHandler<GetPaymentsByCustomerIdRequest, List<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IValidator<GetPaymentsByCustomerIdRequest> _validator;

        public GetPaymentsByCustomerIdRequestHandler(IPaymentRepository paymentRepository, IValidator<GetPaymentsByCustomerIdRequest> validator)
        {
            _paymentRepository = paymentRepository;
            _validator = validator;
        }

        public async Task<List<PaymentDto>> Handle(GetPaymentsByCustomerIdRequest request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var payments = await _paymentRepository.GetByCustomerIdAsync(request.CustomerId);

            return payments.Select(payment => new PaymentDto
            {
                Id = payment.Id,
                CustomerId = payment.CustomerId,
                CartId = payment.CartId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = payment.TransactionId,
                CreatedDate = payment.CreatedDate,
                CompletedDate = payment.CompletedDate,
                FailureReason = payment.FailureReason
            }).ToList();
        }
    }
}
