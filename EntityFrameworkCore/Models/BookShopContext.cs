using EntityFrameworkCore.Mapping;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.Models;

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
            
            modelBuilder.ApplyConfiguration(new Book_CategoryMap());

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
        public DbSet<EntityFrameworkCore.Models.Translator> Translator { get; set; } = default!;
        public DbSet<EntityFrameworkCore.Models.Publisher> Publisher { get; set; } = default!;
    }
}
