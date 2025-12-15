using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LanguagesController : Controller
    {
        private readonly IUnitOfWork _UW;
        public LanguagesController(IUnitOfWork unitOfWork)
        {
            _UW = unitOfWork;
        }

        // GET: Admin/Languages
        public async Task<IActionResult> Index()
        {
            return View(await _UW.BaseRepository<Language>().FindAllAsync());
        }

        // GET: Admin/Languages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = await _UW.BaseRepository<Language>().FindById(id);
            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        // GET: Admin/Languages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Languages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Language language)
        {
            try
            {
                _UW.BaseRepository<Language>().Create(language);
                await _UW.Commit();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(language);
            }

        }

        // GET: Admin/Languages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = await _UW.BaseRepository<Language>().FindById(id);
            if (language == null)
            {
                return NotFound();
            }
            return View(language);
        }

        // POST: Admin/Languages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LanguageID,LanguageName")] Language language)
        {
            if (id != language.LanguageID)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    _UW.BaseRepository<Language>().Update(language);
                    await _UW.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LanguageExists(language.LanguageID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(language);
        }

        // GET: Admin/Languages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = await _UW.BaseRepository<Language>().FindById(id);
            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        // POST: Admin/Languages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var language = await _context.Languages.FindAsync(id);
            //if (language != null)
            //{
            //    _context.Languages.Remove(language);
            //}
            //await _context.SaveChangesAsync();
            var language = await _UW.BaseRepository<Language>().FindById(id);
            if (language == null)
            {
                return NotFound();
            }
            else
            {
                _UW.BaseRepository<Language>().Delete(language);
                _UW.Commit();
                return RedirectToAction(nameof(Index));
            }
        }

        private bool LanguageExists(int id)
        {
            return _UW._Context.Languages.Any(e => e.LanguageID == id);
        }
    }
}
