using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using order_service.Application.Features.Order.Commands.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Application
{
    public static class AppcalitonServiceRegistration
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<CreateOrderCommand>();
            });

            services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();

            return services;
        }
    }
}
