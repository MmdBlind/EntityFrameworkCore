using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Encodings.Web;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController(IApplicationUserManager userManager,SignInManager<ApplicationUser> signInManager,UrlEncoder urlEncoder) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user=await userManager.GetUserAsync(User);
            if(user==null)
            {
                return NotFound();
            }
            else
            {
                var unFormatedKey=await userManager.GetAuthenticatorKeyAsync(user);
                if(string.IsNullOrEmpty(unFormatedKey))
                {
                    await userManager.ResetAuthenticatorKeyAsync(user);
                    unFormatedKey = await userManager.GetAuthenticatorKeyAsync(user);
                }
                enableAuthenticatorViewModel viewModel = new enableAuthenticatorViewModel
                {
                    AuthenticatorUri = GenerateQrCodeUri(unFormatedKey,user.Email),
                    SharedKey=FormatKey(unFormatedKey)
                };
                return View(viewModel);
            }
        }

        public string FormatKey(string unFormatedKey)
        {
            var resaults = new StringBuilder();
            int currentPosition = 0;
            while(currentPosition+4<unFormatedKey.Length)
            {
                resaults.Append(unFormatedKey.Substring(currentPosition, 4));
                resaults.Append(" ");
                currentPosition += 4;
            }
            if(currentPosition<unFormatedKey.Length)
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
