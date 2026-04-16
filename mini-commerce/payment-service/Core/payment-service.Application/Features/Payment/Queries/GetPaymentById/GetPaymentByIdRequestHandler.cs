using FluentValidation;
using MediatR;
using payment_service.Application.Dtos;
using payment_service.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Features.Payment.Queries.GetPaymentById
{
    public class GetPaymentByIdRequestHandler : IRequestHandler<GetPaymentByIdRequest, PaymentDto>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IValidator<GetPaymentByIdRequest> _validator;

        public GetPaymentByIdRequestHandler(IPaymentRepository paymentRepository, IValidator<GetPaymentByIdRequest> validator)
        {
            _paymentRepository = paymentRepository;
            _validator = validator;
        }

        public async Task<PaymentDto> Handle(GetPaymentByIdRequest request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var payment = await _paymentRepository.GetByIdAsync(request.Id);
            if (payment == null)
            {
                throw new Exception("Ödeme bulunamadı.");
            }

            return new PaymentDto
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
            };
        }
    }
}
