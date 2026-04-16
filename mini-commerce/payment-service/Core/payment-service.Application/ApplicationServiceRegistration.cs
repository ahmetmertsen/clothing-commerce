using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using payment_service.Application.Features.Payment.Commands.ProcessPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application
{
    public static class ApplicaitonServiceRegistration
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<ProcessPaymentCommandHandler>();
            });

            services.AddValidatorsFromAssemblyContaining<ProcessPaymentCommandValidator>();

            return services;
        }
    }
}
