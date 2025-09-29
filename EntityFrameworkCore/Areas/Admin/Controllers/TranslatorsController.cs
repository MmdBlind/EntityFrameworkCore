using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TranslatorsController : Controller
    {
        private readonly BookShopContext _context;
        public TranslatorsController(BookShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult>  Index()
        {
            return View(await _context.Translator.ToListAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TranslatorsCreateViewModel ViewModel)
        {
            try
            { 
                Translator translator = new Translator()
                {
                    FirstName = ViewModel.FirstName,
                    LastName = ViewModel.LastName
                };  
                _context.Translator.Add(translator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(ViewModel );
            }
        }

    }
}
