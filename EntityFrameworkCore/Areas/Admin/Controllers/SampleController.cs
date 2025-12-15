using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SampleController : Controller
    {
        private readonly IUnitOfWork _UW;
        public SampleController(IUnitOfWork unitOfWork)
        {
            _UW = unitOfWork;
        }
        public IActionResult Index()
        {
            Stopwatch Sw=new Stopwatch();
            Sw.Start();
            var Query = EF.CompileAsyncQuery((BookShopContext Context,int id)
                => Context.Books.SingleOrDefault(b=>b.BookID==id));
            for(int i=1;i<1000;i++)
            {
                var book = Query(_UW._Context, i);
            }
            Sw.Stop();
            return View(Sw.ElapsedMilliseconds);
        }
    }
}
