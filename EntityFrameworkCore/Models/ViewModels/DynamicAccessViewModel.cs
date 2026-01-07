using EntityFrameworkCore.Areas.Identity.Data;

namespace EntityFrameworkCore.Models.ViewModels
{
    public class DynamicAccessIndexViewModel
    {

        public string[] ActionIds { get; set; }

        public string RoleId { get; set; }

        public ApplicationRole RoleIncludeRoleClaims { get; set; }

        public ICollection<ControllerViewModel> SecuredControllerActions { get; set; }
    
    }
}
