using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Classes;
using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Encodings.Web;

namespace EntityFrameworkCore.Controllers
{
    public class AccountController(IApplicationRoleManager roleManager, IApplicationUserManager userManager, IEmailSender emailSender, BookShopContext context, SignInManager<ApplicationUser> signInManager) : Controller
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
                        await roleManager.CreateAsync(new ApplicationRole("کاربر", "مشتری سایت"));
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

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel viewModel)
        {
            if (Captcha.ValidateCaptchaCode(viewModel.CaptchaCode, HttpContext))
            {


                if (ModelState.IsValid)
                {
                    var Resault = await signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, viewModel.RememberMe, false);
                    if (Resault.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور شما صحیح نمی‌باشد.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "کد امنیتی صحیح نمی‌باشد.");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var resault = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", resault.CaptchaCode);
            Stream s = new MemoryStream(resault.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }
    }
}
