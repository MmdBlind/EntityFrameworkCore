using System.Diagnostics;
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
            if(id!=null)
            {
                ViewBag.ConfirmEmailAlert = "لینک فعالسازی حساب کاربری به ایمیل شما ارسال شد لطفا با کلیک روی این لینک حساب  خود را فعال کنید.";
            }
            return View();
        }

    }
}
