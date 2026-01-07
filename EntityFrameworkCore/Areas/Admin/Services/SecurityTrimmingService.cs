using EntityFrameworkCore.Areas.Admin.Data;
using System.Security.Claims;

namespace EntityFrameworkCore.Areas.Admin.Services
{
    public class SecurityTrimmingService(IHttpContextAccessor httpContextAccessor, IMvcActionsDiscoveryService mvcActionsDiscoveryService) : ISecurityTrimmingService
    {


        public bool CanCurrentUserAccess(string area, string controller, string action)
        {
            return httpContextAccessor.HttpContext != null && CanUserAccess(httpContextAccessor.HttpContext.User, area, controller, action);
        }

        public bool CanUserAccess(ClaimsPrincipal user, string area, string controller, string action)
        {
            var currentClaimValue = $"{area}:{controller}:{action}";
            var securedControllerActions = mvcActionsDiscoveryService.GetAllSecuredControllerActionsWithPolicy(ConstantPolicies.DynamincPermission);
            if (!securedControllerActions.SelectMany(x => x.MvcActions).Any(x => x.ActionId == currentClaimValue))
            {
                throw new KeyNotFoundException($@"The `secured` area={area}/controller={controller}/action={action} with `ConstantPolicies.DynamicPermission` policy not found. Please check you have entered the area/controller/action names correctly and also it's decorated with the correct security policy.");
            }

            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            return user.HasClaim(claim => claim.Type == ConstantPolicies.DynamincPermission&&
                                          claim.Value == currentClaimValue);
        }
    }
}
