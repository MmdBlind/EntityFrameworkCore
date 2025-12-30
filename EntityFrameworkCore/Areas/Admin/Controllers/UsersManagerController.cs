using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersManagerController : Controller
    {
        private readonly IApplicationUserManager _userManager;
        public UsersManagerController(IApplicationUserManager userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string Msg, int page = 1,int row=10)
        {
            if(Msg=="Success")
            {
                ViewBag.Alert = "عضویت با موفقیت انجام شد.";
            }
            var PagingModel = PagingList.Create(await _userManager.GetAllUsersWithRolesAsync(), row, page);
            return View(PagingModel);
        }
    }
}
