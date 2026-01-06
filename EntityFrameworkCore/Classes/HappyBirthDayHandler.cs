using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EntityFrameworkCore.Classes
{
    public class HappyBirthDayHandler : AuthorizationHandler<HappyBirthDayRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HappyBirthDayRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
            {
                return Task.CompletedTask;
            }
            else
            {
                var dateOfBirth = Convert.ToDateTime(context.User.FindFirstValue(ClaimTypes.DateOfBirth));
                if (dateOfBirth.ToString("MM/dd") == DateTime.Now.ToString("MM/dd"))
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
        }
    }
}
