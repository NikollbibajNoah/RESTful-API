using System.Net;
using System.Text.Json;
using RESTful.Exceptions;

namespace RESTful.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        } catch (Exception e) {
            _logger.LogError(e, "Unhandled exception occured");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await HandleExceptionAsync(context, e);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        
        string message = "An unexpected error occured";

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                _logger.LogWarning(exception, message);
                break;
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                _logger.LogWarning(exception, message);
                break;
            case DatabaseException:
                statusCode = HttpStatusCode.InternalServerError;
                message = exception.Message;
                _logger.LogError(exception, message);
                break;
            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "An unexpected error occured";
                _logger.LogError(exception, message);
                break;
            // More exceptions down here...
        } 
        
        var errorDetails = new ErrorDetails
        {
            StatusCode = (int)statusCode,
            Message = message,
#if DEBUG
            Details = exception.StackTrace
#endif
        };
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails));
    }
}