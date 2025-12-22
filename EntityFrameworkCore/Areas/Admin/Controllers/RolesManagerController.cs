using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesManagerController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index(int page = 1, int row = 10)
        {
            var Roles = _roleManager.Roles.Select(r => new RolesViewModel
            {
                RoleID = r.Id,
                RoleName = r.Name
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
                var resault = await _roleManager.CreateAsync(new IdentityRole(viewModel.RoleName));
                if(resault.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.Error = "در ذخیره اطلاعات هطایی رخ داده است.";
                return View(viewModel );
            }
            return View(viewModel);
        }
        
        public async Task<IActionResult> EditRole(string? id)
        {
            if(id is null)
            {
                return NotFound();
            }
            var Role = await _roleManager.FindByIdAsync(id);
            if(Role is null)
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
    }
}
