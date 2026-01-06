using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Areas.Identity.Services;
using EntityFrameworkCore.Classes;
using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text.Encodings.Web;
using static System.Net.WebRequestMethods;

namespace EntityFrameworkCore.Controllers
{
    public class AccountController(IApplicationRoleManager roleManager, IApplicationUserManager userManager, IEmailSender emailSender, BookShopContext context, SignInManager<ApplicationUser> signInManager, ISmsSender smsSender, IConfiguration configuration, IHttpClientFactory httpClientFactory,IConvertDate convertDate) : Controller
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
                DateTime birthDateMiladi = convertDate.ConvertShamsiToMiladi(viewModel.BirthDate);
                var Transaction = context.Database.BeginTransaction();
                var User = new ApplicationUser
                {
                    UserName = viewModel.UserName,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    RegisterDate = DateTime.Now,
                    IsActive = true,
                    BirthDate = birthDateMiladi
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
                    await userManager.AddClaimAsync(User, new Claim(ClaimTypes.DateOfBirth,birthDateMiladi.ToString("MM/dd")));
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
        public IActionResult SignIn(string ReturnUrl=null)
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
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            user.LastVisitDateTime = DateTime.Now;
            await userManager.UpdateAsync(user);
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
            var factorOptions = new List<SelectListItem>();
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var userFactors = await userManager.GetValidTwoFactorProvidersAsync(user);
                foreach (var item in userFactors)
                {
                    if (item == "Authenticator")
                    {
                        factorOptions.Add(new SelectListItem { Text = "اپلیکیشن احراز هویت", Value = item });
                    }
                    else
                    {
                        factorOptions.Add(new SelectListItem { Text = (item == "Email" ? "ارسال ایمیل" : "ارسال پیامک"), Value = item });
                    }
                }
                SendCodeViewModel viewModel = new SendCodeViewModel
                {
                    Providers = factorOptions,
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
                if (viewModel.SelectedProvider != "Authenticator")
                {
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
                else
                {
                    return RedirectToAction("LoginWith2FA", new { rememberMe = viewModel.RememberMe });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoginWith2FA(bool rememberMe)
        {
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                LoginWith2FAViewModel viewModel = new LoginWith2FAViewModel
                {
                    RememberMe = rememberMe
                };
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2FA(LoginWith2FAViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            else
            {
                var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    var authenticationCode = viewModel.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
                    var resault = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticationCode, viewModel.RememberMe, viewModel.RememberMachine);
                    if (resault.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (resault.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 20 دقیقه مسدود می‌باشد.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "کد اعتبار سنجی شما نامعتبر است.");
                    }
                    return View(viewModel);
                }
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
            var resault = await signInManager.TwoFactorSignInAsync(viewModel.Provider, viewModel.code, viewModel.RememberMe, viewModel.RememberBrowser);
            if (resault.Succeeded)
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                UserSidebarViewModel sidebarVM = new UserSidebarViewModel
                {
                    FullName = user.FirstName + " " + user.LastName,
                    RegisterDate = user.RegisterDate,
                    LastVisit = user.LastVisitDateTime,
                    Image = user.Image
                };
                ChangePasswordViewModel changePasswordVM = new ChangePasswordViewModel
                {
                    UserSidebar = sidebarVM
                };
                return View(changePasswordVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            var user = await userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    var changePasswordResault = await userManager.ChangePasswordAsync(user, viewModel.OldPassword, viewModel.NewPassword);
                    if (changePasswordResault.Succeeded)
                    {
                        ViewBag.Alert = "کلمه عبور با موفقیت تغییر یافت.";
                    }
                    else
                    {
                        foreach (var error in changePasswordResault.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    UserSidebarViewModel sidebarVM = new UserSidebarViewModel
                    {
                        FullName = user.FirstName + " " + user.LastName,
                        RegisterDate = user.RegisterDate,
                        LastVisit = user.LastVisitDateTime,
                        Image = user.Image
                    };
                    viewModel.UserSidebar = sidebarVM;
                    return View(viewModel);
                }
            }
            else
            {
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoginWithRecoveryCode()
        {
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return NotFound();
            }
            var rcoveryCode = viewModel.RecoveryCode.Replace(" ", string.Empty);
            var resault = await signInManager.TwoFactorRecoveryCodeSignInAsync(rcoveryCode);
            if (resault.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (resault.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 20 دقیقه مسدود می‌باشد.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "کد اعتبارسنجی وارد شده صحیح نمی‌باشد.");
            }
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetExternalLoginProvider(string provider)
        {
            if (provider == "Yahoo")
            {
                var client_id = configuration.GetValue<string>("YahooOAuth:ClientID");
                //for host we can use this
                //var yahooRedirectUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/signin-yahoo";
                return Redirect($"https://api.login.yahoo.com/oauth2/request_auth?client_id={client_id}&redirect_uri=https://messier-lecia-twopenny.ngrok-free.dev/yahoo-signin&response_type=code&language=en-us");
            }
            var redirectUrl = Url.Action("GetCallBackAsync", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [ActionName("GetCallBackAsync")]
        public async Task<IActionResult> GetCallBackAsync()
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            var resault = await ExternalLoginAsync(info);
            if (resault == "Success")
            {
                return RedirectToAction("Index", "Home");
            }
            else if (resault == "RequiersTwoFactor")
            {
                return RedirectToAction("SendCode");
            }
            else
            {
                ModelState.AddModelError(string.Empty, resault);
                return View("SignIn");
            }
        }

        [Route("yahoo-signin")]
        public async Task<IActionResult> GetYahooCallBackAsync(string code, string state)
        {
            HttpClient httpClient = new HttpClient();

            //for host we use this
            //var yahooRedirectUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/signin-yahoo";

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("client_id", configuration.GetValue<string>("YahooOAuth:ClientID"));
            param.Add("client-secret", configuration.GetValue<string>("YahooOAuth:ClientSecret"));

            //for host we use this
            //param.Add("redirect_uri", yahooRedirectUrl);

            param.Add("redirect_uri", "https://messier-lecia-twopenny.ngrok-free.dev/yahoo-signin");
            param.Add("code", code);
            param.Add("grant_type", "authorization_code");

            FormUrlEncodedContent formatContent = new FormUrlEncodedContent(param);

            var response = await httpClient.PostAsync("https://api.login.yahoo.com/oauth2/get_token", formatContent);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                dynamic jsonData = JsonConvert.DeserializeObject(jsonResponse);

                Console.WriteLine(jsonData);

                string token = jsonData.access_token;
                string guid = jsonData.xoauth_yahoo_guid;
                var request = new HttpRequestMessage(HttpMethod.Get, "https://social.yahooapi.com/v1/user/me/profile?format=json");
                request.Headers.Add("Authorization", $"Bearer{token}");
                var myClient = httpClientFactory.CreateClient();
                response = await myClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    jsonResponse = response.Content.ReadAsStringAsync().Result;
                    jsonData = JsonConvert.DeserializeObject(jsonResponse);

                    string email = jsonData.profile.emails[0].handle;
                    var claim = new List<Claim> { new Claim(ClaimTypes.Email, email) };
                    var claimIdentity = new ClaimsIdentity(claim);
                    var info = new ExternalLoginInfo(new ClaimsPrincipal(claimIdentity), "Yahoo", guid, "Yahoo");
                    var resault = await ExternalLoginAsync(info);
                    if (resault == "Success")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (resault == "RequiersTwoFactor")
                    {
                        return RedirectToAction("SendCode");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, resault);
                        return View("SignIn");
                    }
                }
            }
            ModelState.AddModelError(string.Empty,"در ورود به سایت از طریق اکانت یاهو خطایی رخ داده است.");
            return View("SignIn");
        }

        public async Task<string> ExternalLoginAsync(ExternalLoginInfo info)
        {
            if (info == null)
            {
                return $"در عملیات ورود به سایت از طریق حساب {info.ProviderDisplayName} خطایی رخ داده است.";
            }
            var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return "شما عضو سایت نیستید برای ورود به سایت باید عضو سایت شوید.";
            }
            else
            {
                var resault = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
                if (resault.Succeeded)
                {
                    return "Success";
                }
                else if (resault.IsLockedOut)
                {
                    return "حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شد.";
                }
                else if (resault.RequiresTwoFactor)
                {
                    return "RequiersTwoFactor";
                }
                else
                {
                    var externalResault = await userManager.AddLoginAsync(user, info);
                    if (externalResault.Succeeded)
                    {
                        await signInManager.SignInAsync(user, false);
                        return "Success";
                    }
                    else
                    {
                        return $"در عملیات ورود به سایت از طریق حساب {info.ProviderDisplayName} خطایی رخ داده است.";
                    }
                }
            }
        }

        public IActionResult AccessDenied(string ReturnUrl=null)
        {
            return View();
        }

        [Authorize(policy:"HappyBirthDay")]
        public IActionResult HappyBirthDay()
        {
            return View();
        }
    }
}