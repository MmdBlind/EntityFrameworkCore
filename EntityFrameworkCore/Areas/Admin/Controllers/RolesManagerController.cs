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
        public IActionResult Index(int page=1,int row=10)
        {
            var Roles= _roleManager.Roles.Select(r=>new RolesViewModel 
            {
                RoleID=r.Id,
                RoleName=r.Name
            }).ToList();
            var PagingModel = PagingList.Create(Roles,row,page);
            PagingModel.RouteValue=new RouteValueDictionary
            {
                {"row",row}
            };
            return View(PagingModel);
        }
    }
}
