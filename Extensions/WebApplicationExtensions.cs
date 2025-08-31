using StringDiff.Middlewares;

namespace StringDiff.Extensions;

public static class WebApplicationExtensions
{
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