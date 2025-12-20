using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Models.ViewModels
{
    public class RolesViewModel
    {
        public string RoleID { get; set; }
        
        [Display(Name = "عنوان نقش")]
        public string RoleName { get; set; }
    }
}
