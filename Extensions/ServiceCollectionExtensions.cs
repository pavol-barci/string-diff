using Microsoft.AspNetCore.Mvc;
using StringDiff.Application;
using StringDiff.Application.Services;
using StringDiff.Infrastructure;
using StringDiff.Infrastructure.Repositories;
using StringDiff.Infrastructure.Repositories.InMemory;
using StringDiff.Middlewares;
using StringDiff.Models;

namespace StringDiff.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDiffService, DiffService>();
        services.AddScoped<IDiffCalculator, DiffCalculator>();

        return services;
    }

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