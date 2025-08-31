using System.Diagnostics;

namespace StringDiff.Middlewares;

public class LoggingMiddleware(ILogger<LoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = new Stopwatch();
        
        logger.LogInformation("Calling endpoint {method} {endpoint}", context.Request.Method, context.Request.Path);
        
        stopwatch.Start();
        await next.Invoke(context);
        stopwatch.Stop();
        
        logger.LogInformation("Resulted status code {statusCode} in {elapsed} ms", context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}