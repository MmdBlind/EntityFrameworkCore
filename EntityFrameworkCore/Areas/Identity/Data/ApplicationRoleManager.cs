
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

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


        
    }
}
