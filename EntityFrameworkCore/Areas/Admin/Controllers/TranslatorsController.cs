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
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var Translator = await _context.Translator.FirstOrDefaultAsync(m => m.TranslatorID == id);
                if (Translator == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(Translator);
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Translator translator)
        {
            try
            {
                _context.Update(translator);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(translator);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var Translator = await _context.Translator.FirstOrDefaultAsync(m => m.TranslatorID == id);
                if (Translator == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(Translator);
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deleted(int TranslatorID)
        {
            var Translator = await _context.Translator.FirstAsync(m => m.TranslatorID == TranslatorID);
            _context.Translator.Remove(Translator);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
