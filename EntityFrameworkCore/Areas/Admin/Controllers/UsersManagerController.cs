using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    public class UsersManagerController : Controller
    {
        [Area("Admin")]
        public IActionResult Index(string Msg)
        {
            if(Msg=="Success")
            {
                ViewBag.Alert = "عضویت با موفقیت انجام شد.";
            }
            return View();
        }
    }
}
