using FluentValidation;
using MediatR;
using payment_service.Application.Repositories;
using payment_service.Domain.Entities;
using payment_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Features.Payment.Commands.ProcessPayment
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, ProcessPaymentCommandResponse>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IValidator<ProcessPaymentCommand> _validator;

        public ProcessPaymentCommandHandler(IPaymentRepository paymentRepository, IValidator<ProcessPaymentCommand> validator)
        {
            _paymentRepository = paymentRepository;
            _validator = validator;
        }

        public async Task<ProcessPaymentCommandResponse> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var payment = new Domain.Entities.Payment(request.CustomerId, request.CartId, request.Amount, request.PaymentMethod);
            
            if (request.SimulateSuccess == true)
            {
                payment.Status = Domain.Enums.PaymentStatus.Succeeded;
                payment.CompletedDate = DateTime.UtcNow;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = "Ödeme işlemi başarısız oldu.";
                payment.CompletedDate = DateTime.UtcNow;
            }

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return new ProcessPaymentCommandResponse
            {
                Succeeded = payment.Status == PaymentStatus.Succeeded,
                Message = payment.Status == PaymentStatus.Succeeded ? "Ödeme başarıyla tamamlandı." : "Ödeme başarısız oldu",
                PaymentId = payment.Id,
                TransactionId = payment.TransactionId,
                Status = payment.Status.ToString()
            };
        }
    }
}
