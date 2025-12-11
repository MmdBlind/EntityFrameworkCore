using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkCore.Models
{
    public class Book
    {
        public ILazyLoader LazyLoader { get; set; }
        private Publisher _Publisher;
        private Language _Language;
        public Book()
        { }
        public Book(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Summery { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public string? File { get; set; } = null;
        public byte[]? Image { get; set; } = null;
        public int LanguageID { get; set; }
        public int NumOfPages { get; set; }
        public short Wheight { get; set; }
        public string ISBN { get; set; }
        public bool IsPublish { get; set; }
        public DateTime? PublishDate { get; set; } = null;
        public int PublishYear { get; set; }
        public bool IsDelete { get; set; }
        public int PublisherID { get; set; }


        public Publisher Publisher
        {
            get => LazyLoader.Load(this, ref _Publisher);
            set => _Publisher = value;
        }
        public int SCategoryID { get; set; }


        public Language Language
        {
            get => LazyLoader.Load(this, ref _Language);
            set => _Language = value;
        }
        public Discount Discount { get; set; }
        public List<Author_Book> Author_Book { get; set; }
        public List<Order_Book> Order_Book { get; set; }
        public List<Translator_Book> Translator_Books { get; set; }
        public List<Book_Category> book_Categories { get; set; }
    }
    public class Book_Category
    {
        public int BookID { get; set; }
        public int CategoryID { get; set; }

        public Book Book { get; set; }
        public Category Category { get; set; }
    }
    public class Author
    {
        private ILazyLoader LazyLoader { get; set; }
        private List<Author_Book> _Author_Book;
        public Author()
        {
        }
        private Author(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

        [Key]
        public int AuthorID { get; set; }

        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }


        public List<Author_Book> Author_Book
        {
            //get; set;
            get => LazyLoader.Load(this, ref _Author_Book);
            set => _Author_Book = value;
        }
    }
    public class Author_Book
    {
        public ILazyLoader LazyLoader { get; set; }
        private Book _Book;
        public Author_Book()
        {

        }
        private Author_Book(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        public int BookID { get; set; }
        public int AuthorID { get; set; }
        public Book Book
        {
            get => LazyLoader.Load(this, ref _Book);
            set => _Book = value;
        }
        public Author Author { get; set; }
    }
    public class Discount
    {
        public int BookID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public byte Percent { get; set; }


        public Book Book { get; set; }
    }
    public class Language
    {
        [Key]
        public int LanguageID { get; set; }
        public string LanguageName { get; set; }
        public List<Book> Books { get; set; }
    }
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCategoryID { get; set; }
        public Category category { get; set; }
        public List<Category> categories { get; set; }
        public List<Book_Category> book_Categories { get; set; }

    }
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Mobile { get; set; }
        public string Tellephone { get; set; }
        public string Image { get; set; }

        public int Age { get; set; }

        public City City1 { get; set; }
        public City City2 { get; set; }
        public List<Order> Orders { get; set; }
        public int cityID1 { get; set; }
        public int cityID2 { get; set; }

    }
    public class Provice
    {
        [Key]
        public int ProviceID { get; set; }

        [Display(Name = "نام استان")]
        public string ProviceName { get; set; }

        public List<City> Citys { get; set; }
    }
    public class City
    {
        [Key]
        public int CityID { get; set; }
        public string CityName { get; set; }

        public Provice Provice { get; set; }

        public List<Customer> Customers1 { get; set; }

        public List<Customer> Customers2 { get; set; }
    }
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public long AmountPaid { get; set; }
        public string DispatchNumber { get; set; }
        public DateTime BuyDate { get; set; }


        public OrderStatus OrderStatus { get; set; }
        public Customer Customer { get; set; }
        public List<Order_Book> Order_Books { get; set; }
    }
    public class OrderStatus
    {
        [Key]
        public int OrderStatusID { get; set; }
        public int OrderStatusName { get; set; }


        public List<Order> Orders { get; set; }

    }
    public class Order_Book
    {
        public int BookID { get; set; }
        public int OrderID { get; set; }
        public Order Order { get; set; }
        public Book Book { get; set; }

    }
    public class Publisher
    {
        public int PublisherID { get; set; }
        public string PublisherName { get; set; }

        public List<Book> Books { get; set; }
    }
    public class Translator
    {
        [Key]
        public int TranslatorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Translator_Book> Translator_Books { get; set; }
    }
    public class Translator_Book
    {
        public int BookID { get; set; }
        public int TranslatorID { get; set; }
        public Book Book { get; set; }
        public Translator Translator { get; set; }
    }
}
