using EntityFrameworkCore.Mapping;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Models
{
    public class BookShopContext : DbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=(local);Database=BookShopDB;Trusted_Connection=True;TrustServerCertificate=True");

        //}
        public BookShopContext(DbContextOptions<BookShopContext> options)
            : base (options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Author_BookMap());

            modelBuilder.ApplyConfiguration(new BookMap());

            modelBuilder.ApplyConfiguration(new CustomerMap());

            modelBuilder.ApplyConfiguration(new CityMap());

            modelBuilder.ApplyConfiguration(new ProviceMap());

            modelBuilder.ApplyConfiguration(new DiscountMap());

            modelBuilder.ApplyConfiguration(new Order_BookMap());

            modelBuilder.ApplyConfiguration(new CategoryMap());

            modelBuilder.ApplyConfiguration(new Book_TranslatorMap());

        }
        DbSet<Book> Books { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<Author> Authors { get; set; }
        DbSet<City> Citys { get; set; }
        DbSet<Provice> Provices { get; set; }
        DbSet<Author_Book> Author_Books { get; set; }
        DbSet<Order_Book> Order_Books { get; set; }
        DbSet<Language> Languages { get; set; }
        DbSet<Discount> Discounts { get; set; }
        DbSet<OrderStatus> OrderStatuses { get; set; }
        DbSet<Customer> Customers { get; set; }
    }
}
