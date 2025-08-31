using StringDiff.Middlewares;

namespace StringDiff.Extensions;

/// <summary>
/// Collection of extension methods which configure specific parts of the application services
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configure core web api services (swagger, middlewares, etc) 
    /// </summary>
    /// <param name="app">Current web application</param>
    /// <returns>Current web application</returns>
    public static WebApplication AddCore(this WebApplication app)
    {
        app.AddSwagger()
            .UseHttpsRedirection()
            .UseAuthorization();
        
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();
        
        app.MapControllers();


        return app;
    }

    /// <summary>
    /// Configure swagger based on encironment 
    /// </summary>
    /// <param name="app">Current web application</param>
    /// <returns>Current web application</returns>
    private static WebApplication AddSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}