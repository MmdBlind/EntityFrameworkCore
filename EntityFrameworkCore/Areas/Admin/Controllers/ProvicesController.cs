using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProvicesController : Controller
    {
        private readonly IUnitOfWork _UW;
        public ProvicesController(IUnitOfWork unitOfWork)
        {
            _UW = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _UW.BaseRepository<Provice>().FindAllAsync());
        }
    }
}