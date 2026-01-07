using Microsoft.AspNetCore.Identity;

namespace EntityFrameworkCore.Areas.Identity.Data
{
    public class ApplicationRoleClaim:IdentityRoleClaim<string>
    {
        public virtual ApplicationRole Role {  get; set; }
    }
}
