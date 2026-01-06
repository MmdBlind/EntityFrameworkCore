using EntityFrameworkCore.Models.ViewModels;

namespace EntityFrameworkCore.Areas.Admin.Services
{
    public interface IMvcActionsDiscoveryService
    {
        ICollection<ControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName);
    }
}
