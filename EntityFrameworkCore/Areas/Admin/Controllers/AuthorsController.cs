using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Mono.TextTemplating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorsController : Controller
    {
        private readonly IUnitOfWork _UW;

        public AuthorsController(IUnitOfWork UW)
        {
            _UW = UW;
        }

        // GET: Admin/Authors
        public async Task<IActionResult> Index()
        {
            var Author = _UW.BaseRepository<Author>().FindAllAsync();
            return View(Author);
        }

        // GET: Admin/Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = _UW.BaseRepository<Author>().FindById(id);

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Admin/Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorID,FirstName,LastName")] Author author)
        {
            if (ModelState.IsValid)
            {
                await _UW.BaseRepository<Author>().Create(author);
                await _UW.Commit();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Admin/Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _UW.BaseRepository<Author>().FindById(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Admin/Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorID,FirstName,LastName")] Author author)
        {
            if (id != author.AuthorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _UW.BaseRepository<Author>().Update(author);
                    await _UW.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_UW.BaseRepository<Author>().FindById(author.AuthorID) == null)
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
            return View(author);
        }

        // GET: Admin/Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _UW.BaseRepository<Author>().FindById(id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Admin/Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _UW.BaseRepository<Author>().FindById(id);
            if (author != null)
            {
                _UW.BaseRepository<Author>().Delete(author);
            }

            await _UW.Commit();
            return RedirectToAction(nameof(Index));
        }

        private List<EntityStatus> DisplayStates(IEnumerable<EntityEntry> entities)
        {
            List<EntityStatus> StateInfo = new List<EntityStatus>();
            foreach (var entry in entities)
            {
                EntityStatus States = new EntityStatus()
                {
                    EntityName = entry.Entity.GetType().Name,
                    EntityState = entry.State.ToString()
                };
                StateInfo.Add(States);
            }
            return StateInfo;
        }
        public async Task<IActionResult> AuthorBooks(int id)
        {
            var Author = await _UW.BaseRepository<Author>().FindById(id);
            if (Author == null)
            {
                return NotFound();
            }
            else
            {
                return View(Author);
            }

        }
    }
}
