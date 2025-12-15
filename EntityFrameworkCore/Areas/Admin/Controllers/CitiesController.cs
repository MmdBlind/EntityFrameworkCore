using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CitiesController : Controller
    {
        private readonly IUnitOfWork _UW;
        public CitiesController(IUnitOfWork unitOfWork)
        {
            _UW = unitOfWork;
        }
        public async Task<IActionResult> Index(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            else
            {
                var provice = await _UW._Context.Provices.SingleAsync(p => p.ProviceID == id);
                _UW._Context.Entry(provice).Collection(p => p.Citys).Load();
                return View(provice);
            }
        }
    }
}
