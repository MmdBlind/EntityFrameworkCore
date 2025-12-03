using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.Repository;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using ReflectionIT.Mvc.Paging;
using System.Net.WebSockets;
using static System.Reflection.Metadata.BlobBuilder;

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
        public IActionResult Index(string Msg, int pageindex = 1, int row = 5, string sortExpression = "Title", string title = "")
        {
            if (Msg == "Faild")
            {
                ViewBag.Msg = "در ثبت اطلاعات خطایی رخ داده است لطفا مجددا تلاش کنید!!!";
            }
            else if (Msg == "Success")
            {
                ViewBag.Msg = "عملیات با موفقیت انجام شد.";
            }
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
            BooksCreateEditViewModel viewModel = new BooksCreateEditViewModel(_repository.GetAllCategories());
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BooksCreateEditViewModel viewModel)
        {
            try
            {
                try
                {
                    List<Translator_Book> translators = new List<Translator_Book>();
                    List<Book_Category> categories = new List<Book_Category>();
                    if (viewModel.TranslatorID != null)
                        translators = viewModel.TranslatorID.Select(a => new Translator_Book { TranslatorID = a }).ToList();

                    if (viewModel.CategoryID != null)
                        categories = viewModel.CategoryID.Select(a => new Book_Category { CategoryID = a }).ToList();

                    var transaction = _context.Database.BeginTransaction();
                    DateTime? PublishDate = null;
                    if (viewModel.IsPublish == true)
                        PublishDate = DateTime.Now;

                    Book book = new Book()
                    {
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
                        Author_Book = viewModel.AuthorID.Select(a => new Author_Book { AuthorID = a }).ToList(),
                        Translator_Books = translators,
                        book_Categories = categories
                    };
                    await _context.Books.AddAsync(book);

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    return RedirectToAction("index", new { Msg = "Faild" });
                }
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
            var BookInfo = _context.ReadAllBooks.Where(b => b.BookID == id).First();
            return View(BookInfo);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                book.IsDelete = true;
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                else
                {
                    var viewModel = await _context.Books
                                                .Where(b => b.BookID == id)
                                                .Select(b => new BooksCreateEditViewModel
                                                {
                                                    BookID = b.BookID,
                                                    Title = b.Title,
                                                    Summary = b.Summery,
                                                    Price = b.Price,
                                                    Stock = b.Stock,
                                                    File = b.File,
                                                    NumOfPages = b.NumOfPages,
                                                    Weight = b.Wheight,
                                                    ISBN = b.ISBN,
                                                    IsPublish = b.IsPublish,
                                                    RecentIsPublish = b.IsPublish,
                                                    PublishYear = b.PublishYear,
                                                    PublishDate=b.PublishDate,
                                                    LanguageID = b.LanguageID,
                                                    PublisherID = b.PublisherID,
                                                    AuthorID = b.Author_Book
                                                                 .Where(c => c.BookID == b.BookID)
                                                                 .Select(a => a.AuthorID)
                                                                 .ToArray(),
                                                    TranslatorID = b.Translator_Books
                                                                    .Where(t => t.BookID == b.BookID)
                                                                    .Select(t => t.TranslatorID).ToArray(),
                                                    CategoryID = b.book_Categories
                                                                    .Where(c => c.BookID == b.BookID)
                                                                    .Select(c => c.CategoryID)
                                                                    .ToArray()
                                                })
                                                .FirstAsync();
                    ViewBag.LanguageID = new SelectList(_context.Languages, "LanguageID", "LanguageName");
                    ViewBag.PublisherID = new SelectList(_context.Publisher, "PublisherID", "PublisherName");
                    ViewBag.AuthorID = new SelectList(_context.Authors.Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + " " + t.LastName }), "AuthorID", "NameFamily");
                    ViewBag.TranslatorID = new SelectList(_context.Translator.Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.FirstName + " " + t.LastName }), "TranslatorID", "NameFamily");
                    viewModel.Categories = _repository.GetAllCategories();

                    return View(viewModel);
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BooksCreateEditViewModel viewModel)
        {
            ViewBag.LanguageID = new SelectList(_context.Languages, "LanguageID", "LanguageName");
            ViewBag.PublisherID = new SelectList(_context.Publisher, "PublisherID", "PublisherName");
            ViewBag.AuthorID = new SelectList(_context.Authors.Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + " " + t.LastName }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(_context.Translator.Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.FirstName + " " + t.LastName }), "TranslatorID", "NameFamily");
            viewModel.Categories = _repository.GetAllCategories();

            try
            {
                var transaction = _context.Database.BeginTransaction();
                DateTime? PublishDate;
                if (viewModel.RecentIsPublish == true && viewModel.IsPublish == false)
                {
                    PublishDate =null;
                }
                else if (viewModel.RecentIsPublish == false && viewModel.IsPublish == true)
                { 
                    PublishDate= DateTime.Now;
                }
                else
                {
                    PublishDate = viewModel.PublishDate;
                }
                    Book book = new Book()
                    {
                        BookID = viewModel.BookID,
                        Title = viewModel.Title,
                        Summery = viewModel.Summary,
                        Price = viewModel.Price,
                        Stock = viewModel.Stock,
                        File = viewModel.File,
                        NumOfPages = viewModel.NumOfPages,
                        Wheight = viewModel.Weight,
                        ISBN = viewModel.ISBN,
                        IsPublish = viewModel.IsPublish,
                        PublishYear = viewModel.PublishYear,
                        PublishDate=PublishDate,
                        LanguageID = viewModel.LanguageID,
                        PublisherID = viewModel.PublisherID,
                    };
                _context.Update(book);
                var RecentTranslators = _context.Translator_Books
                                            .Where(t => t.BookID == viewModel.BookID)
                                            .Select(b => b.TranslatorID).ToArray();
                var RecentAuthors = _context.Author_Books
                                        .Where(t => t.BookID == viewModel.BookID)
                                        .Select(b => b.AuthorID)
                                        .ToArray();
                var RecentCategories = _context.Book_Categories
                                           .Where(t => t.BookID == viewModel.BookID)
                                           .Select(b => b.CategoryID)
                                           .ToArray();
                var inputTranslatorIDs = viewModel.TranslatorID ?? new int[0];
                var inputAuthorIDs = viewModel.AuthorID ?? new int[0];
                var inputCategoryIDs = viewModel.CategoryID ?? new int[0];

                var deletedTranslators = RecentTranslators.Except(inputTranslatorIDs);
                var deletedAuthors = RecentAuthors.Except(inputAuthorIDs);
                var deletedCategories = RecentCategories.Except(inputCategoryIDs);

                var ToAddTranslators = inputTranslatorIDs.Except(RecentTranslators);
                var ToAddAuthors = inputAuthorIDs.Except(RecentAuthors);
                var ToAddCategories = inputCategoryIDs.Except(RecentCategories);

                #region اضافه کردن و پاک کردن مترجم و نویسنده و دسته بندی به دیتابیس
                if (deletedTranslators.Count() != 0)
                    _context.Translator_Books.RemoveRange(deletedTranslators.Select(t => new Translator_Book { TranslatorID = t, BookID = viewModel.BookID }).ToList());

                if (deletedAuthors.Count() != 0)
                    _context.Author_Books.RemoveRange(deletedAuthors.Select(a => new Author_Book { AuthorID = a, BookID = viewModel.BookID }).ToList());

                if (deletedCategories.Count() != 0)
                    _context.Book_Categories.RemoveRange(deletedCategories.Select(c => new Book_Category { CategoryID = c, BookID = viewModel.BookID }).ToList());

                if (ToAddTranslators.Count() != 0)
                    _context.Translator_Books.AddRange(ToAddTranslators.Select(t => new Translator_Book { TranslatorID = t, BookID = viewModel.BookID }).ToList());

                if (ToAddAuthors.Count() != 0)
                    _context.Author_Books.AddRange(ToAddAuthors.Select(a => new Author_Book { AuthorID = a, BookID = viewModel.BookID }).ToList());

                if (ToAddCategories.Count() != 0)
                    _context.Book_Categories.AddRange(ToAddCategories.Select(c => new Book_Category { CategoryID = c, BookID = viewModel.BookID }).ToList());
                #endregion

                await _context.SaveChangesAsync();
                transaction.Commit();

                return RedirectToAction("index");
            }
            catch
            {
                ViewBag.Msg = "اطلاعات فرم نامعتبر است.";
                return View("Edit");

            }



        }
    }
}
