 
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Areas.Identity.Data
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole> , IApplicationRoleManager
    {
        private readonly IdentityErrorDescriber _errors;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly ILogger<ApplicationRoleManager> _logger;
        private readonly IEnumerable<IRoleValidator<ApplicationRole>> _rolevalidators;
        private readonly IRoleStore<ApplicationRole> _store;
        public ApplicationRoleManager(IRoleStore<ApplicationRole> store, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<ApplicationRoleManager> logger, IEnumerable<IRoleValidator<ApplicationRole>> roleValidators) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _store = store;
            _keyNormalizer = keyNormalizer;
            _errors = errors;
            _rolevalidators = roleValidators;
            _logger = logger;
        }
        public List<ApplicationRole> GetAllRoles()
        {
            return Roles.ToList();
        }
        public List<RolesViewModel> GetAllRolesAndUsersCount()
        {
            return Roles.Select(role => new RolesViewModel
            {
                RoleID = role.Id,
                RoleName = role.Name,
                RoleDescription = role.Description,
                UsersCount = role.Users.Count()
            }).ToList();
        }

        public Task<ApplicationRole> FindClaimsInRole(string roleId)
        {
            return Roles.Include(c => c.Claims).FirstOrDefaultAsync(c=>c.Id==roleId);
        }

        public async Task<IdentityResult> AddOrUpdateClaimsAsync(string roleId,string roleClaimType,IList<string> selectedRoleClaimValues)
        {
            var role = await FindClaimsInRole(roleId);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError 
                {
                    Code="NotFound",
                    Description="نقش مورد نظر یافت نشد"
                });
            }
            var currentRoleClaimValues = role.Claims.Where(r => r.ClaimType == roleClaimType).Select(r => r.ClaimValue).ToList();
            if (currentRoleClaimValues == null)
            {
                currentRoleClaimValues = new List<string>();
            }
            var newClaimValuesToAdd = selectedRoleClaimValues.Except(currentRoleClaimValues);
            foreach(var claim in newClaimValuesToAdd)
            {
                role.Claims.Add(new ApplicationRoleClaim
                {
                    RoleId = roleId,
                    ClaimType = roleClaimType,
                    ClaimValue = claim
                });
            }
            var removedClaimValues = currentRoleClaimValues.Except(selectedRoleClaimValues).ToList();
            foreach (var claim in removedClaimValues)
            {
                var roleClaim=role.Claims.SingleOrDefault(r=>r.ClaimValue==claim && r.ClaimType==roleClaimType);
                if(roleClaim != null)
                {
                    role.Claims.Remove(roleClaim);
                }
            }
            return await UpdateAsync(role);
        }
        
    }
}
