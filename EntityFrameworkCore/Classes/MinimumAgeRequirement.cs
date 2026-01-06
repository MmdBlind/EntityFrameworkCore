using Microsoft.AspNetCore.Authorization;

namespace EntityFrameworkCore.Classes
{
    public class MinimumAgeRequirement:IAuthorizationRequirement
    {
        public MinimumAgeRequirement(int _minimumAge)
        {
            Age = _minimumAge;
        }
        public int Age { get; set; }
    }
}
