using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Mapping;
using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.Models
{
    public class BookShopContext : IdentityDbContext<BookShopUser,ApplicationRole,string>
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=(local);Database=BookShopDB;Trusted_Connection=True;TrustServerCertificate=True");

        //}
        public BookShopContext(DbContextOptions<BookShopContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationRole>().ToTable("AspNetRoles").ToTable("AppRoles");

            modelBuilder.ApplyConfiguration(new Author_BookMap());

            modelBuilder.ApplyConfiguration(new BookMap());

            modelBuilder.ApplyConfiguration(new CustomerMap());

            modelBuilder.ApplyConfiguration(new CityMap());

            modelBuilder.ApplyConfiguration(new ProviceMap());

            modelBuilder.ApplyConfiguration(new DiscountMap());

            modelBuilder.ApplyConfiguration(new Order_BookMap());

            modelBuilder.ApplyConfiguration(new CategoryMap());

            modelBuilder.ApplyConfiguration(new Book_TranslatorMap());

            modelBuilder.ApplyConfiguration(new Book_CategoryMap());
            modelBuilder
            .Entity<ReadAllBook>()
            .HasNoKey()
            .ToView("ReadAllBooks");

            //Global query filter Example 
            modelBuilder.Entity<Book>().HasQueryFilter(f => f.IsDelete == false);
        }
        public DbSet<Book> Books { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<City> Citys { get; set; }
        public DbSet<Provice> Provices { get; set; }
        public DbSet<Author_Book> Author_Books { get; set; }
        public DbSet<Order_Book> Order_Books { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Translator> Translator { get; set; }
        public DbSet<Publisher> Publisher { get; set; }
        public DbSet<Book_Category> Book_Categories { get; set; }
        public DbSet<Translator_Book> Translator_Books { get; set; }
        public DbSet<ReadAllBook> ReadAllBooks { get; set; }

        [DbFunction("GetAllAuthors","dbo")]
        public static String  GetAllAuthors(int BookID)
        { 
            throw new NotImplementedException();
        }

        [DbFunction("GetAllCategories", "dbo")]
        public static String GetAllCategories(int BookID)
        {
            throw new NotImplementedException();
        }

        [DbFunction("GetAllTranslators", "dbo")]
        public static String GetAllTranslators(int BookID)
        {
            throw new NotImplementedException();
        }

    }
}
