using EntityFrameworkCore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace EntityFrameworkCore.Models.Repository
{
    public class BooksRepository
    {
        private readonly BookShopContext _context;
        public BooksRepository(BookShopContext context)
        {
            _context = context;
        }

        public List<TreeViewCategory> GetAllCategories()
        {
            var Categories = (from c in _context.Categories
                              where (c.ParentCategoryID == null)
                              select new TreeViewCategory { id = c.CategoryID, title = c.CategoryName }).ToList();
            foreach (var item in Categories)
            {
                BindSubCategories(item);
            }
            return Categories;
        }

        public void BindSubCategories(TreeViewCategory category)
        {
            var SubCategories = (from c in _context.Categories
                                 where (c.ParentCategoryID == category.id)
                                 select new TreeViewCategory { id = c.CategoryID, title = c.CategoryName }).ToList();
            foreach (var item in SubCategories)
            {
                BindSubCategories(item);
                category.subs.Add(item);
            }

        }
        public List<BooksIndexViewModel> GetAllBooks(string title, string ISBN, string Language, string Publisher, string Author, string Translator, string Category)
        {


            string AuthorsName = "";
            string CategoryName = "";
            string translatorName = "";

            List<BooksIndexViewModel> ViewModel = new List<BooksIndexViewModel>();
            var Books = (from u in _context.Author_Books
                         .Include(b => b.Book)
                         .ThenInclude(c => c.Publisher)
                         .Include(a => a.Author)
                         join s in _context.Translator_Books on u.Book.BookID equals s.BookID into bt
                         from bts in bt.DefaultIfEmpty()
                         join t in _context.Translator on bts.TranslatorID equals t.TranslatorID into tr
                         from trl in tr.DefaultIfEmpty()
                         join r in _context.Book_Categories on u.Book.BookID equals r.BookID into bc
                         from bct in bc.DefaultIfEmpty()
                         join c in _context.Categories on bct.CategoryID equals c.CategoryID into cg
                         from cog in cg.DefaultIfEmpty()
                         where (u.Book.IsDelete == false && u.Book.Title.Contains(title.Trim()) &&
                                u.Book.ISBN.Contains(ISBN.Trim()) &&
                                u.Book.Language.LanguageName.Contains(Language.Trim()) &&
                                u.Book.Publisher.PublisherName.Contains(Publisher.Trim()))
                         select new
                         {
                             Author = u.Author.FirstName + " " + u.Author.LastName,
                             Translator = bts != null ? trl.FirstName + " " + trl.LastName :"",
                             Category = bct != null ? cog.CategoryName:"",
                             u.Book.BookID,
                             u.Book.ISBN,
                             u.Book.IsPublish,
                             u.Book.Price,
                             u.Book.PublishDate,
                             u.Book.Publisher.PublisherName,
                             u.Book.Stock,
                             u.Book.Title,
                             u.Book.Language.LanguageName
                         })
                         .Where(a => a.Author.Contains(Author) &&
                                a.Translator.Contains(Translator) &&
                                a.Category.Contains(Category))
                         .AsEnumerable()
                         .GroupBy(b => b.BookID)
                         .Select(g => new { BookID = g.Key, BookGroups = g })
                         .ToList();
            foreach (var item in Books)
            {
                AuthorsName = "";
                translatorName = "";
                CategoryName = "";

                foreach (var group in item.BookGroups.Select(a=>a.Author).Distinct())
                {
                    if (AuthorsName == null)
                    {
                        AuthorsName = group;
                    }
                    else
                    {
                        AuthorsName = AuthorsName + " - " + group;
                    }
                }
                foreach (var group in item.BookGroups.Select(a => a.Translator).Distinct())
                {
                    if (translatorName == null)
                    {
                        translatorName = group;
                    }
                    else
                    {
                        translatorName = translatorName + " - " + group;
                    }
                }
                foreach (var group in item.BookGroups.Select(a => a.Category).Distinct())
                {
                    if (CategoryName == null)
                    {
                        CategoryName = group;
                    }
                    else
                    {
                        CategoryName = CategoryName + " - " + group;
                    }
                }
                BooksIndexViewModel VM = new BooksIndexViewModel()
                {
                    Translator=translatorName,
                    Category=CategoryName,
                    Author = AuthorsName,
                    BookID = item.BookID,
                    ISBN = item.BookGroups.First().ISBN,
                    IsPublish = item.BookGroups.First().IsPublish,
                    Price = item.BookGroups.First().Price,
                    Title = item.BookGroups.First().Title,
                    PublishDate = item.BookGroups.First().PublishDate,
                    Stock = item.BookGroups.First().Stock,
                    PublisherName = item.BookGroups.First().PublisherName,
                    Language=item.BookGroups.First().LanguageName
                };
                ViewModel.Add(VM);
            }
            return ViewModel;
        }
    }
}
