using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Encodings.Web;

namespace EntityFrameworkCore.Controllers
{
    public class AccountController(IApplicationRoleManager roleManager, IApplicationUserManager userManager, IEmailSender emailSender, BookShopContext context) : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var Transaction = context.Database.BeginTransaction();
                var User = new ApplicationUser
                {
                    UserName = viewModel.UserName,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    RegisterDate = DateTime.Now,
                    IsActive = true
                };
                IdentityResult Resault = await userManager.CreateAsync(User, viewModel.Password);
                if (Resault.Succeeded)
                {
                    var Role = await roleManager.FindByNameAsync("کاربر");
                    if (Role == null)
                    {
                        await roleManager.CreateAsync(new ApplicationRole("کاربر","مشتری سایت"));
                    }
                    Resault = await userManager.AddToRoleAsync(User, "کاربر");
                    Transaction.Commit();
                    if (Resault.Succeeded)
                    {
                        var Code = await userManager.GenerateEmailConfirmationTokenAsync(User);
                        var CallbackUrl = Url.Action("ConfirmEmail", "Account", values: new { UserId = User.Id, Code = Code }, protocol: Request.Scheme);

                        await emailSender.SendEmailAsync(User.Email, "تایید ایمیل حساب کاربری سایت گل ممد", $"<div dir='rtl' style='font-family:tahoma;font-size:14px'>لطفا با کلیک روی لینک روبرو ایمیل خود را تایید کنید.<a href='{HtmlEncoder.Default.Encode(CallbackUrl)}'>کلیک کنید</a><div>");

                        return RedirectToAction("Index", "Home", new { id = "ConfirmEmail" });
                    }

                }
                foreach (var error in Resault.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var User = await userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return NotFound($"Unable To Load User With Id'{userId}'");
            }
            var Resault = await userManager.ConfirmEmailAsync(User, code);
            if (!Resault.Succeeded)
            {
                throw new InvalidOperationException($"Error Confirming Email For UserWithId:'{userId}'");
            }
            return View();
        }
    }
}
