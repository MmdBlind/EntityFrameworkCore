using System.Diagnostics;
using System.Security.Claims;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string id)
        {
            //var UserInfo = User;
            //string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //string UserRole== User.FindFirstValue(ClaimTypes.Role);

            if (id!=null)
            {
                ViewBag.ConfirmEmailAlert = "لینک فعالسازی حساب کاربری به ایمیل شما ارسال شد لطفا با کلیک روی این لینک حساب  خود را فعال کنید.";
            }
            return View();
        }

    }
}
