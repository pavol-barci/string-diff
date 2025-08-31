using System.Diagnostics;

namespace StringDiff.Middlewares;

/// <summary>
/// Middleware to globally log information about current request
/// </summary>
/// <param name="logger">Logger instance for the middleware</param>
public class LoggingMiddleware(ILogger<LoggingMiddleware> logger) : IMiddleware
{
    /// <summary>
    /// Handle current middleware in the request pipeline. Measure the request time
    /// and log information about the time, endpoint and response status code
    /// </summary>
    /// <param name="context">Current http context</param>
    /// <param name="next">The delegate representing the remaining middleware in the request pipeline</param>
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