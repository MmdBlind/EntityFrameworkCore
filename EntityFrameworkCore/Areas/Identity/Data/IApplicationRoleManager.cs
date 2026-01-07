using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace EntityFrameworkCore.Areas.Identity.Data
{
    public interface IApplicationRoleManager
    {
        #region BaseClass

        IQueryable<ApplicationRole> Roles { get; }

        ILookupNormalizer KeyNormalizer { get; set; }

        IdentityErrorDescriber ErrorDescriber { get; set; }

        IList<IRoleValidator<ApplicationRole>> RoleValidators { get; }

        bool SupportsQueryableRoles { get; }

        bool SupportsRoleClaims { get; }

        Task<IdentityResult> CreateAsync(ApplicationRole role);

        Task<IdentityResult> DeleteAsync(ApplicationRole role);

        Task<ApplicationRole> FindByIdAsync(string roleId);

        Task<ApplicationRole> FindByNameAsync(string name);
        
        string NormalizeKey(string key);

        Task<bool> RoleExistsAsync(string name);

        Task<IdentityResult> UpdateAsync(ApplicationRole role);

        Task UpdateNormalizedRoleNameAsync(ApplicationRole role);

        Task<string> GetRoleNameAsync(ApplicationRole role);

        Task<IdentityResult> SetRoleNameAsync(ApplicationRole role, string name);

        #endregion

        #region CustomeMethod

        List<ApplicationRole> GetAllRoles();

        List<RolesViewModel> GetAllRolesAndUsersCount();

        Task<ApplicationRole> FindClaimsInRole(string roleId);

        Task<IdentityResult> AddOrUpdateClaimsAsync(string roleId, string roleClaimType, IList<string> selectedRoleClaimValues);

        #endregion
    }
}
