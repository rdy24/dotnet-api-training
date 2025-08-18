using System.Net;
using System.Text.Json;
using CinemaApi.Responses;

namespace CinemaApi.Middleware
{
    public class UnauthorizedResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UnauthorizedResponseMiddleware> _logger;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public UnauthorizedResponseMiddleware(RequestDelegate next, ILogger<UnauthorizedResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Handle unauthorized and forbidden responses
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync(context);
            }
            else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                await HandleForbiddenAsync(context);
            }
        }

        private async Task HandleUnauthorizedAsync(HttpContext context)
        {
            _logger.LogWarning("Unauthorized access attempt to {Path}", context.Request.Path);

            context.Response.ContentType = "application/json";
            
            var errorResponse = new ErrorResponse
            {
                Success = false,
                Message = "Unauthorized access",
                Details = "Authentication token is missing or invalid",
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse, JsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }

        private async Task HandleForbiddenAsync(HttpContext context)
        {
            _logger.LogWarning("Forbidden access attempt to {Path} by user {User}", 
                context.Request.Path, context.User.Identity?.Name ?? "Unknown");

            context.Response.ContentType = "application/json";
            
            var errorResponse = new ErrorResponse
            {
                Success = false,
                Message = "Access forbidden",
                Details = "You don't have permission to access this resource",
                StatusCode = (int)HttpStatusCode.Forbidden,
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse, JsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    // Extension method untuk register middleware
    public static class UnauthorizedResponseMiddlewareExtensions
    {
        public static IApplicationBuilder UseUnauthorizedResponse(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UnauthorizedResponseMiddleware>();
        }
    }
}