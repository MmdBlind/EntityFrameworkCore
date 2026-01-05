using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Encodings.Web;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController(IApplicationUserManager userManager, SignInManager<ApplicationUser> signInManager, UrlEncoder urlEncoder) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var viewModel = await LoadSharedKeyAndQrCodeUriAsync(user);
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel viewModel)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                var enableAuthenticatorVM = await LoadSharedKeyAndQrCodeUriAsync(user);
                return View(enableAuthenticatorVM);
            }
            else
            {
                var verificationCode = viewModel.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
                var is2faTOkenValid = await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
                if (!is2faTOkenValid)
                {
                    ModelState.AddModelError(string.Empty, "کد اعتبار سنجی نامعتبر است.");
                    var enableAuthenticatorVM = await LoadSharedKeyAndQrCodeUriAsync(user);
                    return View(enableAuthenticatorVM);
                }
                await userManager.SetTwoFactorEnabledAsync(user, true);
                if (await userManager.CountRecoveryCodesAsync(user) == 0)
                {
                    var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                    ViewBag.Alert = "اپلیکیشن احراز هویت تایید شد.";
                    return View("ShowRecoveryCodes", recoveryCodes);
                }
                else
                {
                    return RedirectToAction("TwoFactorAuthentication", new {alert="success" });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication(string alert)
        {
            if (alert!=null)
            {
                ViewBag.Alert = "اپلیکیشن احراز هویت تایید شد.";
            }
            return View();
        }

        public async Task<EnableAuthenticatorViewModel> LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
        {
            var unFormatedKey = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unFormatedKey))
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                unFormatedKey = await userManager.GetAuthenticatorKeyAsync(user);
            }
            EnableAuthenticatorViewModel viewModel = new EnableAuthenticatorViewModel
            {
                AuthenticatorUri = GenerateQrCodeUri(unFormatedKey, user.Email),
                SharedKey = FormatKey(unFormatedKey)
            };
            return viewModel;
        }

        public string FormatKey(string unFormatedKey)
        {
            var resaults = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unFormatedKey.Length)
            {
                resaults.Append(unFormatedKey.Substring(currentPosition, 4));
                resaults.Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unFormatedKey.Length)
            {
                resaults.Append(currentPosition);
            }
            return resaults.ToString().ToLowerInvariant();
        }

        public string GenerateQrCodeUri(string unFormatedKey, string email)
        {
            string authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
            return (string.Format(authenticatorUriFormat, urlEncoder.Encode("EntityFrameworkCore"), urlEncoder.Encode(email), unFormatedKey));
        }
    }
}
