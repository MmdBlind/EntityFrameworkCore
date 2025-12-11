using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CitiesController : Controller
    {
        BookShopContext _context;
        public CitiesController(BookShopContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int id)
        {
            if(id==0)
            {
                return NotFound();
            }
            else
            {
                var provice = await _context.Provices.SingleAsync(p => p.ProviceID == id);
                _context.Entry(provice).Collection(p => p.Citys).Load();
                return View(provice);
            }
        }
    }
}
