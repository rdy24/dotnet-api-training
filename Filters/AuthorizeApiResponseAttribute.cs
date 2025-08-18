using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CinemaApi.Responses;

namespace CinemaApi.Filters
{
    public class AuthorizeApiResponseAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                var errorResponse = new ErrorResponse
                {
                    Success = false,
                    Message = "Unauthorized access",
                    Details = "Authentication token is missing or invalid",
                    StatusCode = 401,
                    Timestamp = DateTime.UtcNow,
                    Path = context.HttpContext.Request.Path
                };

                context.Result = new JsonResult(errorResponse) { StatusCode = 401 };
                return;
            }

            // Check roles if specified
            if (!string.IsNullOrEmpty(Roles))
            {
                var userRoles = Roles.Split(',').Select(r => r.Trim());
                var userRole = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userRole) || !userRoles.Contains(userRole))
                {
                    var errorResponse = new ErrorResponse
                    {
                        Success = false,
                        Message = "Access forbidden",
                        Details = "You don't have permission to access this resource",
                        StatusCode = 403,
                        Timestamp = DateTime.UtcNow,
                        Path = context.HttpContext.Request.Path
                    };

                    context.Result = new JsonResult(errorResponse) { StatusCode = 403 };
                    return;
                }
            }
        }
    }
}