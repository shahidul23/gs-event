using System;
using System.Text.Json;
using GSEvent.Common;
using GSEvent.Exceptions;

namespace GSEvent.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred."
            );
            await HandleExceptionAsync(
                context,
                exception
            );
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch { 
            AppException appEx => appEx.StatusCode, 
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized, 
            ArgumentException => StatusCodes.Status400BadRequest, 
            FormatException => StatusCodes.Status400BadRequest, 
            InvalidOperationException => StatusCodes.Status400BadRequest, 
            KeyNotFoundException => StatusCodes.Status404NotFound, 
            TimeoutException => StatusCodes.Status408RequestTimeout, 
            InvalidDataException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        }; 
        // var title = statusCode switch { 
        //     StatusCodes.Status400BadRequest => "Bad Request", 
        //     StatusCodes.Status401Unauthorized => "Unauthorized", 
        //     StatusCodes.Status403Forbidden => "Forbidden", 
        //     StatusCodes.Status404NotFound => "Not Found",
        //     StatusCodes.Status408RequestTimeout => "Request Timeout", 
        //     StatusCodes.Status409Conflict => "Conflict", 
        //     StatusCodes.Status422UnprocessableEntity => "Validation Error", 
        //     StatusCodes.Status429TooManyRequests => "Too Many Requests", 
        //     StatusCodes.Status500InternalServerError => "Internal Server Error", 
        //     StatusCodes.Status502BadGateway => "Bad Gateway", 
        //     StatusCodes.Status503ServiceUnavailable => "Service Unavailable", 
        //     StatusCodes.Status504GatewayTimeout => "Gateway Timeout", 
        //     _ => "An Error Occurred" 
        // };

        var message = exception is AppException appException
            ? appException.Message
            : statusCode == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : exception.Message;
        
        var response = ApiResponse<object>.ErrorResponse(
            message,
            statusCode,
            errors:null
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize( 
            response, 
            new JsonSerializerOptions { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            }
        );
        await context.Response.WriteAsync(json);
    }
}
