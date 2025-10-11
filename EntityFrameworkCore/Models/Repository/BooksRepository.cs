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
            List<BooksIndexViewModel> ViewModel = new List<BooksIndexViewModel>();
            var Books = (from u in _context.Author_Books
                         .Include(b => b.Book)
                         .ThenInclude(c => c.Publisher)
                         .Include(a => a.Author)
                         //join a in _context.Author_Books on u.Book.aut equals a.AuthorID
                         where (u.Book.IsDelete == false && u.Book.Title.Contains(title.Trim()) &&
                                u.Book.ISBN.Contains(ISBN.Trim()) && 
                                u.Book.Language.LanguageName.Contains(Language.Trim()) &&
                                u.Book.Publisher.PublisherName.Contains(Publisher.Trim())
                                )
                         select new
                         {
                             Author = u.Author.FirstName + " " + u.Author.LastName,
                             u.Book.BookID,
                             u.Book.ISBN,
                             u.Book.IsPublish,
                             u.Book.Price,
                             u.Book.PublishDate,
                             u.Book.Publisher.PublisherName,
                             u.Book.Stock,
                             u.Book.Title
                         })
                         .AsEnumerable()
                         .GroupBy(b => b.BookID)
                         .Select(g => new { BookID = g.Key, BookGroups = g })
                         .ToList();
            foreach (var item in Books)
            {
                AuthorsName = "";
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
            return ViewModel;
        }
    }
}
