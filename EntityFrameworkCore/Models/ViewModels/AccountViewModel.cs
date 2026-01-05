using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Models.ViewModels
{
    public class RegisterViewModel:GoogleRecaptchaModelBase
    {
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [EmailAddress(ErrorMessage = "ایمیل شما نامعتبر است.")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string UserName { get; set; }

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PhoneNumber { get; set; }
    }

    public class SignInViewModel
    {
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار؟")]
        public bool RememberMe { get; set; }

        [Required(ErrorMessage ="وارد نمودن {0} الزامی است.")]
        [StringLength(4,ErrorMessage ="کد امنیتی باید دارای 4 کاراکتر باشد.")]
        [Display(Name ="کد امنیتی")]
        public string CaptchaCode { get; set; }
    }

    public class ForgetPasswordViewModel
    {
        [Display(Name ="پست الکترونیک")]
        [EmailAddress(ErrorMessage ="پست الکترونیک وارد شده نامعتبر است.")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [EmailAddress(ErrorMessage = "ایمیل شما نامعتبر است.")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور جدید")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور جدید")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده مطابقت ندارد.")]
        public string ConfirmNewPassword { get; set; }

        public string Code { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }

        public bool RememberMe { get; set; }
    }
    
    public class LoginWith2FAViewModel
    {

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [StringLength(7, ErrorMessage = "کد اعتبارسنجی باید حداقل دارای {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "کد اعتبارسنجی")]
        public string TwoFactorCode { get; set; }


        public bool RememberMe { get; set; }


        [Display(Name = "مرا به خاطر بسپار؟")]
        public bool RememberMachine { get; set; }

    }

    public class  VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required(ErrorMessage ="وارد کردن {0} الزامی است.")]
        [Display(Name ="کد اعتبار سنجی")]
        public string code { get; set; }

        [Display(Name ="مرا به خاطر بسپار؟")]
        public bool RememberBrowser { get; set; }

        [Display(Name ="RememberMe?")]
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Display(Name ="کلمه عبور فعلی")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string OldPassword { get; set; }


        [Display(Name = "کلمه عبور جدید")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        public string NewPassword { get; set; }


        [Display(Name = "تکرار کلمه عبور جدید")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [Compare("NewPassword", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
        [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        public string ConfirmPassword { get; set; }

        public UserSidebarViewModel? UserSidebar { get; set; }
    }

    public class UserSidebarViewModel
    {
        public string FullName { get; set; }
        
        public DateTime? RegisterDate { get; set; }
        
        public DateTime? LastVisit { get; set; }
       
        public string? Image { get; set; }
    }

    public class LoginWithRecoveryCodeViewModel
    {
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [DataType(DataType.Text)]
        [Display(Name = "کد بازیابی")]
        public string RecoveryCode { get; set; }
        
    }
}
