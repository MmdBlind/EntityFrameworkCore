using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Models.ViewModels
{
    public class TranslatorsCreateViewModel
    {
        public int TranslatorID { get; set; }

        [Display(Name ="نام")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است")]
        public string FirstName { get; set; }
         
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage ="وارد نمودن {0} الزامی است")]
        public string LastName { get; set; }
    }
}
