using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Areas.Identity.Services;
using EntityFrameworkCore.Classes;
using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.IdentityModel.Tokens;
using System.Text.Encodings.Web;

namespace EntityFrameworkCore.Controllers
{
    public class AccountController(IApplicationRoleManager roleManager, IApplicationUserManager userManager, IEmailSender emailSender, BookShopContext context, SignInManager<ApplicationUser> signInManager, ISmsSender smsSender) : Controller
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
                    var User = await userManager.FindByNameAsync(viewModel.UserName);
                    if (User.IsActive)
                    {
                        var Resault = await signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, viewModel.RememberMe, true);
                        if (Resault.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        if (Resault.IsLockedOut)
                        {
                            ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شد.");
                            return View();
                        }
                        if (Resault.RequiresTwoFactor)
                        {
                            return RedirectToAction("SendCode", new { rememberMe = viewModel.RememberMe });
                        }
                        ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور شما صحیح نمی‌باشد.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "حساب کاربری شما غیرفعال می‌باشد.لطفا با پشتیبانی سایت تماس بگیرید.");
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

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await userManager.FindByEmailAsync(viewModel.Email);
                if (User == null)
                {
                    ModelState.AddModelError(string.Empty, "ایمیل شما صحیح نمی‌باشد.");
                }
                else
                {
                    if (!await userManager.IsEmailConfirmedAsync(User))
                    {
                        ModelState.AddModelError(string.Empty, "لطفا با تایید ایمیل حساب کاربری خود را فعال کنید.");
                    }
                    else
                    {
                        var Code = await userManager.GeneratePasswordResetTokenAsync(User);
                        var CallbackUrl = Url.Action("ResetPassword", "Account", values: new { Code }, protocol: Request.Scheme);
                        await emailSender.SendEmailAsync(viewModel.Email, "بازیابی کلمه عبور", $"<p style='font-family:tahoma;font-size:14px'>برای بازیابی کلمه عبور خود <a href='{HtmlEncoder.Default.Encode(CallbackUrl)}'>اینجا کلیک کنید</a><p>");
                        return RedirectToAction("ForgetPasswordConfirmation");
                    }
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ForgetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return NotFound();
            }
            else
            {
                var ViewModel = new ResetPasswordViewModel { Code = code };
                return View(ViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await userManager.FindByEmailAsync(viewModel.Email);
                if (User == null)
                {
                    ModelState.AddModelError(string.Empty, "ایمیل شما صحیح نمی‌باشد.");
                }
                else
                {
                    var Resault = await userManager.ResetPasswordAsync(User, viewModel.Code, viewModel.NewPassword);
                    if (Resault.Succeeded)
                    {
                        return RedirectToAction("ResetPasswordConfirmation");
                    }
                    else
                    {
                        foreach (var error in Resault.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public async Task<IActionResult> SendSms()
        {
            string status = await smsSender.SendAuthSmsAsync("5678", "09164145926");
            if (status == "Success")
            {
                ViewBag.Alert = "پیامک با موفقیت ارسال شد.";
            }
            else
            {
                ViewBag.Alert = "خطا در ارسال پیامک.";
            }
            return View();
        }

        public async Task<IActionResult> SendSmsWithPackage()
        {
            List<string> phoneNumbers = new List<string>
            {
                "09164145926"
            };
            string message = "این یک پیامک تستی با استفاده از بسته پیامکی کاوه نگار می‌باشد.";
            string status = await smsSender.SendAuthSmsPackageAsync(phoneNumbers, message);
            if (status == "Success")
            {
                ViewBag.Alert = "پیامک با موفقیت ارسال شد.";
            }
            else if (status == "Failed")
            {
                ViewBag.Alert = "خطا در ارسال پیامک.";
            }
            else if (status == "FailedToConnect")
            {
                ViewBag.Alert = "خطا در برقراری ارتباط با سرور پیامک.";
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SendCode(bool rememberMe)
        {
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var userFactors = await userManager.GetValidTwoFactorProvidersAsync(user);
                userFactors.Remove("Authenticator");
                var factorOptions = userFactors.Select(p => new SelectListItem { Text = (p == "Email" ? "ارسال ایمیل" : "ارسال پیامک"), Value = p }).ToList();
                SendCodeViewModel viewModel = new SendCodeViewModel
                {
                    Providers = factorOptions.ToList(),
                    RememberMe = rememberMe
                };
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    return NotFound();
                }
                var code = await userManager.GenerateTwoFactorTokenAsync(user, viewModel.SelectedProvider);
                if (string.IsNullOrWhiteSpace(code))
                {
                    return View("Error");
                }
                var message = "<p style='direction:rtl;font-size:14px;font-family:tahoma'>کد اعتبار اسنجی شما :" + code + "</p>";
                if (viewModel.SelectedProvider == "Email")
                {
                    await emailSender.SendEmailAsync(user.Email, "کد احراز هویت دو مرحله ای", message);
                }
                else if (viewModel.SelectedProvider == "Phone")
                {
                    string responseSms = await smsSender.SendAuthSmsAsync(code, user.PhoneNumber);
                    if (responseSms == "Failed")
                    {
                        ModelState.AddModelError(string.Empty, "در ارسال پیامک خطایی رخ داده است.");
                        return View(viewModel);
                    }
                }
                return RedirectToAction("VerifyCode", new { provider = viewModel.SelectedProvider, rememberMe = viewModel.RememberMe });
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe)
        {
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return NotFound();
            }
            VerifyCodeViewModel viewModel = new VerifyCodeViewModel
            {
                Provider = provider,
                RememberMe = rememberMe
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var resault = await signInManager.TwoFactorSignInAsync(viewModel.Provider,viewModel.code, viewModel.RememberMe, viewModel.RememberBrowser);
            if(resault.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (resault.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "حساب کاربری شما به دلیل تلاش های ناموفق به مدت 20 دقیقه مسدود شد.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "کد وارد شده صحیح نمی‌باشد.");
            }
            return View(viewModel);
        }
    }
}