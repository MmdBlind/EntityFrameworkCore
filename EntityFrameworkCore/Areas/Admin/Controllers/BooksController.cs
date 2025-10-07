using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.Repository;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace EntityFrameworkCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BooksController : Controller
    {
        private readonly BooksRepository _repository;
        private readonly BookShopContext _context;
        public BooksController(BookShopContext context, BooksRepository repository)
        {
            _context = context;
            _repository = repository;
        }
        public IActionResult Index(int pageindex  = 1)
        {
            List<BooksIndexViewModel> ViewModel = new List<BooksIndexViewModel>();
            string AuthorsName = "";

            //روش زیر روش(eagerlodaing)می باشد و میتوان به جای روش بالا استفاده کرد  
            var Books = (from u in _context.Author_Books
                         .Include(b => b.Book)
                         .ThenInclude(c => c.Publisher)
                         .Include(a => a.Author)
                         where (u.Book.IsDelete == false)
                         select new BooksIndexViewModel
                         {
                             Author = u.Author.FirstName + " " + u.Author.LastName,
                             BookID = u.Book.BookID,
                             ISBN = u.Book.ISBN,
                             IsPublish = u.Book.IsPublish,
                             Price = u.Book.Price,
                             PublishDate = u.Book.PublishDate,
                             PublisherName = u.Book.Publisher.PublisherName,
                             Stock = u.Book.Stock,
                             Title = u.Book.Title
                         })
                         .AsEnumerable()
                         .GroupBy(b => b.BookID)
                         .Select(g => new { BookID = g.Key, BookGroups = g })
                         .ToList();
            foreach (var item in Books)
            {
                AuthorsName = null;
                foreach (var group in item.BookGroups)
                {
                    if (AuthorsName == null)
                    {
                        AuthorsName = group.Author;
                    }
                    else
                    {
                        AuthorsName = AuthorsName + " - " + group.Author;
                    }
                }
                BooksIndexViewModel VM = new BooksIndexViewModel()
                {
                    Author = AuthorsName,
                    BookID = item.BookID,
                    ISBN = item.BookGroups.First().ISBN,
                    IsPublish = item.BookGroups.First().IsPublish,
                    Price = item.BookGroups.First().Price,
                    Title = item.BookGroups.First().Title,
                    PublishDate = item.BookGroups.First().PublishDate,
                    Stock = item.BookGroups.First().Stock,
                    PublisherName = item.BookGroups.First().PublisherName,
                };
                ViewModel.Add(VM);
            }

            var PagingModel = PagingList.Create(ViewModel, 3, pageindex);

            return View(PagingModel);
        }
        public IActionResult Create()
        {
            ViewBag.LanguageID = new SelectList(_context.Languages, "LanguageID", "LanguageName");
            ViewBag.PublisherID = new SelectList(_context.Publisher, "PublisherID", "PublisherName");
            ViewBag.AuthorID = new SelectList(_context.Authors.Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + " " + t.LastName }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(_context.Translator.Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.FirstName + " " + t.LastName }), "TranslatorID", "NameFamily");
            BooksCreateViewModel viewModel = new BooksCreateViewModel(_repository.GetAllCategories());
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BooksCreateViewModel viewModel)
        {
            try
            {
                List<Author_Book> authors = new List<Author_Book>();
                List<Translator_Book> translators = new List<Translator_Book>();
                List<Book_Category> categories = new List<Book_Category>();
                DateTime? PublishDate = null;
                if (viewModel.IsPublish == true)
                {
                    PublishDate = DateTime.Now;
                }
                Book book = new Book()
                {
                    IsDelete = false,
                    ISBN = viewModel.ISBN,
                    IsPublish = viewModel.IsPublish,
                    NumOfPages = viewModel.NumOfPages,
                    Stock = viewModel.Stock,
                    Price = viewModel.Price,
                    LanguageID = viewModel.LanguageID,
                    Summery = viewModel.Summary,
                    Title = viewModel.Title,
                    PublishYear = viewModel.PublishYear,
                    PublishDate = PublishDate,
                    Wheight = viewModel.Weight,
                    PublisherID = viewModel.PublisherID,
                };
                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();
                if (viewModel.AuthorID != null)
                {
                    for (int i = 0; i < viewModel.AuthorID.Length; i++)
                    {
                        Author_Book author = new Author_Book()
                        {
                            BookID = book.BookID,
                            AuthorID = viewModel.AuthorID[i],
                        };
                        authors.Add(author);
                        //await _context.AddAsync(author);
                    }
                    await _context.Author_Books.AddRangeAsync(authors);
                }
                if (viewModel.TranslatorID != null)
                {
                    for (int i = 0; i < viewModel.TranslatorID.Length; i++)
                    {
                        Translator_Book translator = new Translator_Book()
                        {
                            BookID = book.BookID,
                            TranslatorID = viewModel.TranslatorID[i],
                        };
                        translators.Add(translator);
                        //await _context.AddAsync(author);
                    }
                    await _context.Translator_Books.AddRangeAsync(translators);
                }
                if (viewModel.CategoryID != null)
                {
                    for (int i = 0; i < viewModel.CategoryID.Length; i++)
                    {
                        Book_Category category = new Book_Category()
                        {
                            BookID = book.BookID,
                            CategoryID = viewModel.CategoryID[i],
                        };
                        categories.Add(category);
                        //await _context.AddAsync(author);
                    }
                    await _context.Book_Categories.AddRangeAsync(categories);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.LanguageID = new SelectList(_context.Languages, "LanguageID", "LanguageName");
                ViewBag.PublisherID = new SelectList(_context.Publisher, "PublisherID", "PublisherName");
                ViewBag.AuthorID = new SelectList(_context.Authors.Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + " " + t.LastName }), "AuthorID", "NameFamily");
                ViewBag.TranslatorID = new SelectList(_context.Translator.Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.FirstName + " " + t.LastName }), "TranslatorID", "NameFamily");
                viewModel.Categories = _repository.GetAllCategories();
                throw ex;
                return View(viewModel);
            }
        }
    }
}