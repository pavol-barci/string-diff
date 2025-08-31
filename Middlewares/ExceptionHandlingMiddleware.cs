using System.Net;

namespace StringDiff.Middlewares;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
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
        //TODO> if not found,mmissing etc handle in controller, here really handle non common scenarios 

        context.Response.ContentType = "application/json";

        var response = new
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Error = "Error occured."
        };
        
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}