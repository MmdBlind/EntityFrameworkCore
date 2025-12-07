using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SampleController : Controller
    {
        private readonly BookShopContext _context;
        public SampleController(BookShopContext context)
        { 
            _context = context;
        }
        public IActionResult Index()
        {
            Stopwatch Sw=new Stopwatch();
            Sw.Start();
            var Query = EF.CompileAsyncQuery((BookShopContext Context,int id)
                => Context.Books.SingleOrDefault(b=>b.BookID==id));
            for(int i=1;i<1000;i++)
            {
                var book = Query(_context, i);
            }
            Sw.Stop();
            return View(Sw.ElapsedMilliseconds);
        }
    }
}
