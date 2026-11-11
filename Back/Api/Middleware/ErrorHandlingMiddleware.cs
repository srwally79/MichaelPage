using System.Text.Json;
using Domain.Exceptions;

namespace Api.Middleware;

public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var (statusCode, message, details) = exception switch
            {
                BusinessRuleException => (StatusCodes.Status400BadRequest, exception.Message, new[] { exception.Message }),
                EntityNotFoundException => (StatusCodes.Status404NotFound, exception.Message, new[] { exception.Message }),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, exception.Message, new[] { exception.Message }),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", Array.Empty<string>())
            };
            if (statusCode == StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "Unhandled API exception for {Path}", context.Request.Path);

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new ErrorResponse(message, details), new JsonSerializerOptions(JsonSerializerDefaults.Web)),
                context.RequestAborted);
        }
    }

    private sealed record ErrorResponse(string Message, IReadOnlyList<string> Details);
}
