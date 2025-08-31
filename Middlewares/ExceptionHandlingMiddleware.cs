using System.Net;
using StringDiff.Infrastructure.Exceptions;
using StringDiff.Models;

namespace StringDiff.Middlewares;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    private record ErrorWithStatusCode(HttpStatusCode StatusCode, ErrorResponse Body);
    
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