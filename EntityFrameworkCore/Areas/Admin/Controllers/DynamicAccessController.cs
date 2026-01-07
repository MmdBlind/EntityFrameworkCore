using EntityFrameworkCore.Areas.Admin.Data;
using EntityFrameworkCore.Areas.Admin.Services;
using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DynamicAccessController(IApplicationRoleManager roleManager,IMvcActionsDiscoveryService mvcActionsDiscovery) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var role=await roleManager.FindClaimsInRole(id);
            if (role == null)
            { 
                return NotFound();
            }
            var securedControllerActions = mvcActionsDiscovery.GetAllSecuredControllerActionsWithPolicy(ConstantPolicies.DynamincPermission);
            DynamicAccessIndexViewModel viewModel = new DynamicAccessIndexViewModel
            {
                RoleIncludeRoleClaims=role,
                SecuredControllerActions=securedControllerActions
            };
            return View(viewModel);
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DynamicAccessIndexViewModel viewModel)
        {
            var result = await roleManager.AddOrUpdateClaimsAsync(viewModel.RoleId, ConstantPolicies.DynamincPermission, viewModel.ActionIds);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "در حین انجام عملیات خطایی رخ داده است.");
            }
            return RedirectToAction("Index", new {roleId=viewModel.RoleId});
        }
    }
}
