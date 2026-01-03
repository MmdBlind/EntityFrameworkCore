using EntityFrameworkCore.Classes;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkCore.Models
{
    public class GoogleRecaptchaModelBase
    {

        [GoogleRecaptchaValidation]
        [BindProperty(Name ="g-recaptcha-response")]
        public string GoogleRecaptchaResponse { get; set; }
    
    }
}
