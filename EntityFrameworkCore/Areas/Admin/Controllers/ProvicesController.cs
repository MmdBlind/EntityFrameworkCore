using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProvicesController : Controller
    {
        BookShopContext _context;
        public ProvicesController(BookShopContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Provices.ToListAsync());
        }
    }
}