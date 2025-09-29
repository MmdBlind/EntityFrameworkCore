using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    public class TranslatorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
