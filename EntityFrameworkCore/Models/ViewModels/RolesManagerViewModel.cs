using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Models.ViewModels
{
    public class RolesViewModel
    {
        public string? RoleID { get; set; }
        
        [Display(Name = "عنوان نقش")]
        [Required(ErrorMessage ="وارد نمودن {0} الزامی است.")]
        public string RoleName { get; set; }

        [Display(Name ="توضیحات نقش")]
        [Required(ErrorMessage ="وارد نمودن {0} الزامی است")]
        public string RoleDescription { get; set; }

        [Display(Name = "کاربران")]
        public int? UsersCount { get; set; }

        public string? RecentRoleName { get; set; }
    }
}
