
using auth_service.API.Configurations.RateLimiting;
using auth_service.API.Middlewares;
using auth_service.API.Services;
using auth_service.Application;
using auth_service.Application.Abstractions.Services;
using auth_service.Infrastructure;
using auth_service.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace auth_service.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped<IClientContext, HttpClientContext>();
            builder.Services.Configure<SensitiveEndpointRateLimitOptions>(
                builder.Configuration.GetSection("SensitiveEndpointRateLimit"));

            #region Serilog
            Logger log = new LoggerConfiguration()
                .WriteTo.File(
                    path: "logs/authservice-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/errors/error-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 60,
                    restrictedToMinimumLevel: LogEventLevel.Error
                )
                .WriteTo.Seq(builder.Configuration["Seq:ServerURL"])
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .CreateLogger();

            builder.Host.UseSerilog(log);
            #endregion

            #region Swagger
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "authServiceAPI",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Authorization header kullanýmý: token"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
                });
            });
            #endregion

            builder.Services.AddPersistenceService(builder.Configuration);
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationService();

            #region Authentication-Authorization
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        // Doðrulamasý gereken deðerler
                        ValidateAudience = true, //Oluþturulacak token deðerini kimlerin/hangi originlerin/sitelerin kullanýcýðýný belirlediðimiz deðerdir.
                        ValidateIssuer = true, // Oluþturulacak token deðerini kimin daðýttýný ifade edeceðimiz alanýdýr.
                        ValidateLifetime = true, //Oluþturulan token deðerinin süresini kontrol edecek olan doðrulamadýr.
                        ValidateIssuerSigningKey = true, //Üretilecek token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden security key verisinin doðrulanmasýdýr.

                        ValidAudience = builder.Configuration["Token:Audience"],
                        ValidIssuer = builder.Configuration["Token:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
                        NameClaimType = ClaimTypes.Name, //Jwt üzerinden gelen Name claimine karþýlýk gelen deðeri User.Identity.Name propertysinden elde edilir.
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var userIdValue = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                ?? context.Principal?.FindFirst("sub")?.Value;
                            var sessionIdValue = context.Principal?.FindFirst("sid")?.Value
                                ?? context.Principal?.FindFirst(ClaimTypes.Sid)?.Value;

                            if (!Guid.TryParse(userIdValue, out var userId) ||
                                !Guid.TryParse(sessionIdValue, out var sessionId))
                            {
                                context.Fail("Geçerli kullanýcý veya oturum bilgisi bulunamadý.");
                                return;
                            }

                            var authSessionService = context.HttpContext.RequestServices
                                .GetRequiredService<IAuthSessionService>();
                            var isActive = await authSessionService.IsSessionActiveAsync(
                                userId,
                                sessionId,
                                context.HttpContext.RequestAborted);

                            if (!isActive)
                            {
                                context.Fail("Oturum geçersiz veya süresi dolmuþ.");
                            }
                        }
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme
                )
                .RequireAuthenticatedUser()
                .Build();
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();

            app.UseSerilogRequestLogging();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseMiddleware<SensitiveEndpointRateLimitMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
