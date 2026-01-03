// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using EntityFrameworkCore.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Collections;
using EntityFrameworkCore.Classes;

namespace EntityFrameworkCore.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IApplicationRoleManager _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            ILogger<RegisterModel> logger,
            IApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
        }


        [BindProperty]
        public InputModel Input { get; set; }


        [BindProperty]
        public string[] UserRoles { get; set; }


        public IEnumerable<ApplicationRole> GetRoles { get; set; }


        public string ReturnUrl { get; set; }


        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public class InputModel
        {

            [Required(ErrorMessage = "وارد نمودن پست الکترونیک الزامی است.")]
            [EmailAddress(ErrorMessage = "ایمیل شما نامعتبر است.")]
            [Display(Name = "پست الکترونیک")]
            public string Email { get; set; }


            [Required(ErrorMessage = "وارد نمودن گذرواژه الزامی است.")]
            [StringLength(100, ErrorMessage = "{0} حداقل باید {2} کاراکتر و حداکثر {1} کاراکتر باشد.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "گذرواژه")]
            public string Password { get; set; }


            [Required(ErrorMessage = "وارد نمودن تکرار گذرواژه الزامی است.")]
            [DataType(DataType.Password)]
            [Display(Name = "تکرار گذرواژه")]
            [Compare("Password", ErrorMessage = "تکرار گذرواژه با گذرواژه تطابق ندارد")]
            public string ConfirmPassword { get; set; }


            [Display(Name = "نام")]
            [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
            public string Name { get; set; }


            [Display(Name = "نام خانوادگی")]
            [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
            public string Family { get; set; }


            [Display(Name = "تاریخ تولد")]
            [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [PersianDate]
            public string BirthDate { get; set; }


            [Display(Name = "نام کاربری")]
            [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
            public string UserName { get; set; }


            [Display(Name = "شماره موبایل")]
            [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
            public string PhoneNumber { get; set; }

            
            public DateTime RegisterDate { get; set; }


        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            GetRoles = _roleManager.GetAllRoles();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Admin/UsersManager/Index?Msg=Success");
            if (ModelState.IsValid)
            {
                ConvertDate Convert = new ConvertDate();

               var user = new ApplicationUser { UserName = Input.UserName, Email = Input.Email, FirstName = Input.Name, LastName = Input.Family, PhoneNumber = Input.PhoneNumber, BirthDate = Convert.ConvertShamsiToMiladi(Input.BirthDate),IsActive=true, RegisterDate = DateTime.Now,EmailConfirmed=true};

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    if(UserRoles!=null)
                    {
                        await _userManager.AddToRolesAsync(user, UserRoles);

                    }
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            GetRoles= _roleManager.GetAllRoles();
            return Page();
        }

        
    }
}
