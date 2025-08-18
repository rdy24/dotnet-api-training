using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CinemaApi.Attributes
{
    public class RequireRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _role;

        public RequireRoleAttribute(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            
            if (!user.Identity?.IsAuthenticated == true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            
            if (userRole != _role)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}