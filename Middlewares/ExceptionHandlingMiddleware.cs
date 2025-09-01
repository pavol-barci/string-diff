using System.Net;
using StringDiff.Domain.Exceptions;
using StringDiff.Models;

namespace StringDiff.Middlewares;

/// <summary>
/// Middleware to handle all exception and return custom global error response
/// </summary>
/// <param name="logger">Logger instance for the middleware</param>
public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    private record ErrorWithStatusCode(HttpStatusCode StatusCode, ErrorResponse Body);
    
    /// <inheritdoc/>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    /// <summary>
    /// Handle exception. This will log the exception and based on its type
    /// set specific response code with error object from the API
    /// </summary>
    /// <param name="context">Current http context</param>
    /// <param name="exception">Current thrown exception</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {        
        logger.LogError(exception, exception.Message);

        var response = exception switch
        {
            NotFoundException => new ErrorWithStatusCode(HttpStatusCode.NotFound, new ErrorResponse(exception.Message)),
            ConflictException => new ErrorWithStatusCode(HttpStatusCode.Conflict, new ErrorResponse(exception.Message)),
            _ => new ErrorWithStatusCode(HttpStatusCode.InternalServerError, new ErrorResponse(exception.Message))
        };

        context.Response.ContentType = "application/json";        
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsJsonAsync(response.Body);
    }
}