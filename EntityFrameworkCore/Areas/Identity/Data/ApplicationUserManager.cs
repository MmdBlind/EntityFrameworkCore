using EntityFrameworkCore.Areas.Admin.Controllers;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Areas.Identity.Data
{
    public class ApplicationUserManager : UserManager<ApplicationUser> , IApplicationUserManager
    {
        private readonly ApplicationIdentityErrorDescriber _errors;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly ILogger<ApplicationUserManager> _logger;
        private readonly IOptions<IdentityOptions> _options;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IEnumerable<IPasswordValidator<ApplicationUser>> _passwordValidators;
        private readonly IServiceProvider _services;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IEnumerable<IUserValidator<ApplicationUser>> _userValidators;
        public ApplicationUserManager(ApplicationIdentityErrorDescriber errors, ILookupNormalizer keyNormalizer, ILogger<ApplicationUserManager> logger, IOptions<IdentityOptions> options, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, IServiceProvider services, IUserStore<ApplicationUser> userStore, IEnumerable<IUserValidator<ApplicationUser>> userValidators) : base(userStore, options, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _errors = errors;
            _keyNormalizer = keyNormalizer;
            _logger = logger;
            _options = options;
            _passwordHasher = passwordHasher;
            _passwordValidators = passwordValidators;
            _services = services;
            _userStore = userStore;
            _userValidators = userValidators;
        }
        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await Users.ToListAsync();
        }
        public async Task<List<UsersViewModel>> GetAllUsersWithRolesAsync()
        {
            return await Users.Select(user=> new UsersViewModel { 
                Id= user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = user.FirstName,
                Family = user.LastName,
                Image = user.image,
                RegisterDate = user.RegisterDate,
                LastVisitDateTime = user.LastVisitDateTime,
                IsActive = user.IsActive,
                Roles=user.Roles.Select(r=>r.Role.Name),
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled= user.TwoFactorEnabled,
                LockoutEnabled= user.LockoutEnabled,
                EmailConfirmed= user.EmailConfirmed,
                AccessFailedCount= user.AccessFailedCount,
                LockoutEnd= user.LockoutEnd
            }).ToListAsync();
        }

        public string NormalizeKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}
