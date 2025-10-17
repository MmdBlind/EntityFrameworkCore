using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.Repository;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public IActionResult Index(int pageindex = 1, int row = 5, string sortExpression = "Title", string title = "")
        {
            List<int> Rows = new List<int>
            {
                5,10,15,20,50,100
            };
            ViewBag.RowID = new SelectList(Rows, row);
            ViewBag.NumOfRow = (pageindex - 1) * row + 1;
            ViewBag.Search = title;
            string AuthorsName = "";
            title = string.IsNullOrEmpty(title) ? "" : title;

            var PagingModel = PagingList.Create(_repository.GetAllBooks(title, "", "", "", "", "", ""), row, pageindex, sortExpression, "Title");
            PagingModel.RouteValue = new RouteValueDictionary
            {
                {"row",row },
                {"title",title }
            };
            ViewBag.LanguageID = new SelectList(_context.Languages, "LanguageName", "LanguageName");
            ViewBag.PublisherID = new SelectList(_context.Publisher, "PublisherName", "PublisherName");
            ViewBag.AuthorID = new SelectList(_context.Authors.Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + " " + t.LastName }), "NameFamily", "NameFamily");
            ViewBag.TranslatorID = new SelectList(_context.Translator.Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.FirstName + " " + t.LastName }), "NameFamily", "NameFamily");
            ViewBag.Categories = _repository.GetAllCategories();
            return View(PagingModel);
        }
        public IActionResult AdvancedSearch(BooksAdvancedSearch ViewModel)
        {
            ViewModel.Title = string.IsNullOrEmpty(ViewModel.Title) ? "" : ViewModel.Title;
            ViewModel.ISBN = string.IsNullOrEmpty(ViewModel.ISBN) ? "" : ViewModel.ISBN;
            ViewModel.Language = string.IsNullOrEmpty(ViewModel.Language) ? "" : ViewModel.Language;
            ViewModel.Publisher = string.IsNullOrEmpty(ViewModel.Publisher) ? "" : ViewModel.Publisher;
            ViewModel.Author = string.IsNullOrEmpty(ViewModel.Author) ? "" : ViewModel.Author;
            ViewModel.Translator = string.IsNullOrEmpty(ViewModel.Translator) ? "" : ViewModel.Translator;
            ViewModel.Category = string.IsNullOrEmpty(ViewModel.Category) ? "" : ViewModel.Category;
            var Books = _repository.GetAllBooks(ViewModel.Title, ViewModel.ISBN, ViewModel.Language, ViewModel.Publisher, ViewModel.Author, ViewModel.Translator, ViewModel.Category);
            return View(Books);
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

        public IActionResult Details(int id)
        {
            // روش پایین روش SqlRaw است
            //var BookInfo = _context.Books.FromSql($"select * from dbo.BookInfo where BookID={id}")
            //    .Include(l => l.Language)
            //    .Include(p => p.Publisher).First();
            //ViewBag.Authors = _context.Authors.FromSql($"EXEC dbo.GetAuthorsByBookID {id}").ToList();
            //ViewBag.Translators = (from e in _context.Translator_Books
            //                       join t in _context.Translator on e.TranslatorID equals t.TranslatorID
            //                       where (e.BookID == id)
            //                       select new Translator { FirstName = t.FirstName, LastName = t.LastName }).ToArray().ToList();
            //ViewBag.Categories = (from o in _context.Book_Categories
            //                      join c in _context.Categories on o.CategoryID equals c.CategoryID
            //                      where (o.BookID == id)
            //                      select new Category { CategoryName = c.CategoryName }).ToList();
            //return View(BookInfo);
            var BookInfo=_context.ReadAllBooks.Where(b=>b.BookID==id).First();
            return View(BookInfo);
        }
    }
}