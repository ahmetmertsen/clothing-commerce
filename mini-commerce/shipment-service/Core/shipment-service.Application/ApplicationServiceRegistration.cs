using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using shipment_service.Application.Features.Shipment.Commands.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblyContaining<CreateShipmentCommand>());

            services.AddValidatorsFromAssemblyContaining<CreateShipmentCommandValidator>();

            return services;
        }
    }
}
