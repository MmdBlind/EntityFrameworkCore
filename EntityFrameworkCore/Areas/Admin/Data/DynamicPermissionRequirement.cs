using EntityFrameworkCore.Areas.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EntityFrameworkCore.Areas.Admin.Data
{
    public class DynamicPermissionRequirement : IAuthorizationRequirement
    {
    }

    public class DynamicPermissionsAuthorizationHandler(ISecurityTrimmingService securityTrimmingService,IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<DynamicPermissionRequirement>
    {

        protected override Task HandleRequirementAsync(
             AuthorizationHandlerContext context,
             DynamicPermissionRequirement requirement)
        {
            var mvcContext = httpContextAccessor.HttpContext;
            if (mvcContext == null)
            {
                return Task.CompletedTask;
            }

            var actionDescriptor = mvcContext.GetRouteData();

            
            var area = actionDescriptor?.Values["area"]?.ToString();

            var controller = actionDescriptor?.Values["controller"]?.ToString();

            var action = actionDescriptor?.Values["action"]?.ToString();

            if (securityTrimmingService.CanCurrentUserAccess(area, controller, action))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
