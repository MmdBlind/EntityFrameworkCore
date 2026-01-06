using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkCore.Controllers
{
    public class ProductsController : Controller
    {
        [Authorize(policy: "AtLeast18")]
        public IActionResult BannedBooks()
        {
            return View();
        }
    }
}
