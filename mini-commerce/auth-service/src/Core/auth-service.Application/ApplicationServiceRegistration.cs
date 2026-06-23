using auth_service.Application.Common.Behaviors;
using auth_service.Application.Features.Auth.Register;
using auth_service.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;


namespace auth_service.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>();
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(CurrentUserBehavior<,>));
                cfg.AddOpenBehavior(typeof(AccountStatusBehavior<,>));
            });


            services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly);
            services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);

            return services;
        }
    }
}
