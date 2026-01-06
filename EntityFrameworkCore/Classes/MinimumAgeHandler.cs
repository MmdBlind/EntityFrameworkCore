using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EntityFrameworkCore.Classes
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            if(!context.User.HasClaim(c=>c.Type==ClaimTypes.DateOfBirth))
            { 
                return Task.CompletedTask;
            }
            var dateOfBirth = Convert.ToDateTime(context.User.FindFirstValue(ClaimTypes.DateOfBirth));
            int age = DateTime.Today.Year - dateOfBirth.Year;
            if(dateOfBirth>DateTime.Today.AddYears(-age))
            {
                age--;
            }
            if(age>=requirement.Age)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
