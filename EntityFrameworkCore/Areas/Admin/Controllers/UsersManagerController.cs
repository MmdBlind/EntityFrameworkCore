using EntityFrameworkCore.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersManagerController(IApplicationUserManager userManager) : Controller
    {

        public async Task<IActionResult> Index(string Msg, int page = 1, int row = 10)
        {
            if (Msg == "Success")
            {
                ViewBag.Alert = "عضویت با موفقیت انجام شد.";
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
            var UserData = await userManager.FindUsersWithRolesByIdAsync(id);
            if (UserData == null)
            {
                return NotFound();
            }
            else
            {
                return View(UserData);
            }
        }


    }
}
