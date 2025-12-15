using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.UnitOfWork;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TranslatorsController : Controller
    {
        private readonly IUnitOfWork _UW;
        public TranslatorsController(IUnitOfWork unitOfWork)
        {
            _UW = unitOfWork;
        }

        public async Task<IActionResult>  Index()
        {
            return View(await _UW.BaseRepository<Translator>().FindAllAsync());
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
                _UW.BaseRepository<Translator>().Create(translator);
                await _UW.Commit();
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
                var Translator = await _UW.BaseRepository<Translator>().FindById(id);
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
                _UW.BaseRepository<Translator>().Update(translator);
                await _UW.Commit();
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
                var Translator = await _UW.BaseRepository<Translator>().FindById(id);
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
            var Translator = await _UW.BaseRepository<Translator>().FindById(TranslatorID);
            _UW.BaseRepository<Translator>().Delete(Translator);
            await _UW.Commit();
            return RedirectToAction("Index");
        }

    }
}
