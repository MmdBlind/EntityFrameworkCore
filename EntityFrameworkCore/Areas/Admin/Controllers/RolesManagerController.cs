using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesManagerController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public RolesManagerController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index(int page = 1, int row = 10)
        {
            var Roles = _roleManager.Roles.Select(r => new RolesViewModel
            {
                RoleID = r.Id,
                RoleName = r.Name,
                RoleDescription = r.Description
            }).ToList();
            var PagingModel = PagingList.Create(Roles, row, page);
            PagingModel.RouteValue = new RouteValueDictionary
            {
                {"row",row}
            };
            return View(PagingModel);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RolesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (await _roleManager.RoleExistsAsync(viewModel.RoleName))
                {
                    ViewBag.Error = "نام نقش تکراری می‌باشد.";
                    return View();
                }
                else
                {
                    var resault = await _roleManager.CreateAsync(new ApplicationRole(viewModel.RoleName, viewModel.RoleDescription));
                    if (resault.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }

                    ViewBag.Error = "در ذخیره اطلاعات هطایی رخ داده است.";
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            var Role = await _roleManager.FindByIdAsync(id);
            if (Role is null)
            {
                return NotFound();
            }
            RolesViewModel viewModel = new RolesViewModel
            {
                RoleID = Role.Id,
                RoleName = Role.Name,
                RoleDescription = Role.Description
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(RolesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var Role = await _roleManager.FindByIdAsync(viewModel.RoleID);
                if (Role is null)
                {
                    return NotFound();
                }
                if(await _roleManager.RoleExistsAsync(viewModel.RoleName))
                {
                    ViewBag.Message = "نام نقش تکراری می‌باشد.";
                    return View(viewModel);
                }
                else
                {
                    Role.Name = viewModel.RoleName;
                    Role.Description = viewModel.RoleDescription;

                    var Resault = await _roleManager.UpdateAsync(Role);
                    if (Resault.Succeeded is true)
                    {
                        ViewBag.Message = "عملیات با موفقیت انجام شد.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = "در ذخیره اطلاعات خطایی رخ داده است.";
                        return View(viewModel);
                    }
                }
            }
            ViewBag.Error = "اطلاعات وارد شده معتبر نمی باشد.";
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Role = await _roleManager.FindByIdAsync(id);
            if (Role == null)
            {
                return NotFound();
            }
            RolesViewModel viewModel = new RolesViewModel
            {
                RoleID = Role.Id,
                RoleName = Role.Name
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletedRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Role = await _roleManager.FindByIdAsync(id);
            if (Role == null)
            {
                return NotFound();
            }
            var Resault = await _roleManager.DeleteAsync(Role);
            if (Resault.Succeeded)
            {
                ViewBag.Message = "عملیات با موفقیت انجام شد.";
                return RedirectToAction("Index");
            }
            else
            {
                RolesViewModel viewModel = new RolesViewModel
                {
                    RoleID = Role.Id,
                    RoleName = Role.Name
                };

                ViewBag.Message = "عملیات با شکست مواجه شد";

                return View(viewModel);
            }

        }
    }
}
