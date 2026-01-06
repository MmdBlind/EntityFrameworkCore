using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Classes;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using ReflectionIT.Mvc.Paging;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AccessToUsersManager")]
    public class UsersManagerController(IApplicationUserManager userManager, IApplicationRoleManager roleManager, IConvertDate convertDate, IEmailSender emailSender) : Controller
    {

        public async Task<IActionResult> Index(string Msg, int page = 1, int row = 10)
        {
            if (Msg == "Success")
            {
                ViewBag.Alert = "عضویت با موفقیت انجام شد.";
            }
            if (Msg == "SendEmailSuccess")
            {
                ViewBag.Alert = "ارسال ایمیل با موفقیت انجام شد.";
            }
            var PagingModel = PagingList.Create(await userManager.GetAllUsersWithRolesAsync(), row, page);
            return View(PagingModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var UserDetails = await userManager.FindUsersWithRolesByIdAsync(id);
                if (UserDetails == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(UserDetails);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var User = await userManager.FindUsersWithRolesByIdAsync(id);

            if (User == null)
            {
                return NotFound();
            }
            else
            {
                if (User.BirthDate != null)
                {
                    User.PersianBirthDate = convertDate.ConvertMiladiToShamsi((DateTime)User.BirthDate, "yyyy/MM/dd");
                }
                ViewBag.AllRoles = roleManager.GetAllRoles();
                return View(User);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsersViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await userManager.FindByIdAsync(viewModel.Id);
                if (User == null)
                {
                    return NotFound();
                }
                else
                {
                    IdentityResult Resault;
                    var RecentRoles = await userManager.GetRolesAsync(User);
                    var DeletedRoles = RecentRoles.Except(viewModel.Roles);
                    var AddRoles = viewModel.Roles.Except(RecentRoles);

                    Resault = await userManager.RemoveFromRolesAsync(User, DeletedRoles);

                    if (Resault.Succeeded)
                    {
                        Resault = await userManager.AddToRolesAsync(User, AddRoles);
                        if (Resault.Succeeded)
                        {
                            User.UserName = viewModel.UserName;
                            User.FirstName = viewModel.Name;
                            User.LastName = viewModel.Family;
                            User.Email = viewModel.Email;
                            User.PhoneNumber = viewModel.PhoneNumber;
                            if (viewModel.PersianBirthDate != null)
                            {
                                User.BirthDate = convertDate.ConvertShamsiToMiladi(viewModel.PersianBirthDate);
                            }
                            Resault = await userManager.UpdateAsync(User);
                            if (Resault.Succeeded)
                            {
                                ViewBag.AlertSuccess = "ذخیره تغییرات با موفقیت انجام شد.";
                            }
                        }
                    }
                    if (Resault != null)
                    {
                        foreach (var eror in Resault.Errors)
                        {
                            ModelState.AddModelError("", eror.Description);
                        }
                    }
                }
            }
            ViewBag.AllRoles = roleManager.GetAllRoles();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var User = await userManager.FindByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }
            else
            {
                return View(User);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var User = await userManager.FindByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }
            else
            {
                var Resault = await userManager.DeleteAsync(User);
                if (Resault.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.AlertError = "در حذف اطلاعات خطایی رخ داده است.";
                }
                return View(User);
            }
        }

        public async Task<IActionResult> SendEmail(string[] emails, string subject, string message)
        {
            if (emails != null)
            {
                for (int i = 0; i < emails.Length; i++)
                {
                    await emailSender.SendEmailAsync(emails[i], subject, message);
                }
            }
            return RedirectToAction("Index", new { Msg = "SendEmailSuccess" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeLockOutEnable(string userId, bool status)
        {
            var User = await userManager.FindByIdAsync(userId);

            if (User == null)
            {
                return NotFound();
            }
            else
            {
                await userManager.SetLockoutEnabledAsync(User, status);
                return RedirectToAction("Details", new { id = User.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUserAccount(string userId)
        {
            var User = await userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return NotFound();
            }
            else
            {
                await userManager.SetLockoutEndDateAsync(User, DateTimeOffset.UtcNow.AddMinutes(20));
                return RedirectToAction("Details", new { id = User.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnLockUserAccount(string userId)
        {
            var User = await userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return NotFound();
            }
            else
            {
                await userManager.SetLockoutEndDateAsync(User, null);
                return RedirectToAction("Details", new { id = User.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeActiveOrActiveUserAccount(string userId, bool status)
        {
            var User = await userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return NotFound();
            }
            else
            {
                User.IsActive = status;
                await userManager.UpdateAsync(User);
                return RedirectToAction("Details", new { id = User.Id });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            var User = await userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return NotFound();
            }
            else
            {
                var viewModel = new UsersResetPasswordViewModel
                {
                    Id = User.Id,
                    Email = User.Email,
                    UserName = User.UserName,
                };
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(UsersResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await userManager.FindByIdAsync(viewModel.Id);
                if (User == null)
                {
                    return NotFound();
                }
                else
                {
                    IdentityResult resault = await userManager.RemovePasswordAsync(User);

                    if (resault.Succeeded)
                    {
                        resault = await userManager.AddPasswordAsync(User, viewModel.NewPassword);
                        if (resault.Succeeded)
                        {
                            ViewBag.AlertSuccess = "بازنشانی کلمه عبور با موفقیت انجام شد.";
                            viewModel.UserName = User.UserName;
                            viewModel.Email = User.Email;
                        }
                    }
                    foreach (var error in resault.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeTwoFactorEnabled(string userId)
        {
            var User = await userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return NotFound();
            }
            if (User.TwoFactorEnabled == true)
            {
                User.TwoFactorEnabled = false;
            }
            else
            {
                User.TwoFactorEnabled = true;
            }
            var resault = await userManager.UpdateAsync(User);
            if (!resault.Succeeded)
            {
                foreach (var error in resault.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Details", new { id = User.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmailConfirmed(string userId)
        {
            var User = await userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return NotFound();
            }
            if (User.EmailConfirmed == true)
            {
                User.EmailConfirmed = false;
            }
            else
            {
                User.EmailConfirmed = true;
            }
            var resault = await userManager.UpdateAsync(User);
            if (!resault.Succeeded)
            {
                foreach (var error in resault.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Details", new { id = User.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhoneNumberConfimed(string userId)
        {
            var User = await userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return NotFound();
            }
            if (User.PhoneNumberConfirmed)
            {
                User.PhoneNumberConfirmed = false;
            }
            else
            {
                User.PhoneNumberConfirmed = true;
            }
            var resault = await userManager.UpdateAsync(User);
            if (resault.Succeeded)
            {
                foreach (var error in resault.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Details", new { id = User.Id });
        }

    }

}
