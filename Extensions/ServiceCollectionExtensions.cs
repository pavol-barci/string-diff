using Microsoft.AspNetCore.Mvc;
using StringDiff.Application.Abstraction.Services;
using StringDiff.Application.Services;
using StringDiff.Contracts;
using StringDiff.Domain.Repositories;
using StringDiff.Infrastructure.Repositories;
using StringDiff.Infrastructure.Repositories.InMemory;
using StringDiff.Middlewares;

namespace StringDiff.Extensions;

/// <summary>
/// Collection of extension methods which register specific parts of the application services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register application (domain) services
    /// </summary>
    /// <param name="services">Collection of services</param>
    /// <returns>Collection of services</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDiffService, DiffService>();
        services.AddScoped<IDiffCalculator, DiffCalculator>();

        return services;
    }

    /// <summary>
    /// Register infrastructure services
    /// </summary>
    /// <param name="services">Collection of services</param>
    /// <param name="environment">Info about current environment</param>
    /// <returns>Collection of services</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddSingleton<IDiffRepository, DiffInMemoryRepository>();   
        }
        else
        {
            services.AddScoped<IDiffRepository, DiffRepository>();
        }

        return services;
    }

    /// <summary>
    /// Register core (asp.net, swagger, etc) services 
    /// </summary>
    /// <param name="services">Collection of services</param>
    /// <returns>Collection of services</returns>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(ConfigureValidations);
        services.AddEndpointsApiExplorer()
            .AddSwaggerGen();

        services.AddTransient<LoggingMiddleware>();
        services.AddTransient<ExceptionHandlingMiddleware>();

        return services;
    }

    /// <summary>
    /// Configure custom validation handling in order to return custom error object when validation requests
    /// </summary>
    /// <param name="options">Api behaviour options</param>
    private static void ConfigureValidations(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .Select(x => new ValidationError 
                {
                    Property = x.Key,
                    Errors = x.Value?.Errors.Select(e => e.ErrorMessage)
                });

            var validationResponse = new ErrorResponse("Validation of request failed.", errors);

            return new BadRequestObjectResult(validationResponse);
        };
    }
}