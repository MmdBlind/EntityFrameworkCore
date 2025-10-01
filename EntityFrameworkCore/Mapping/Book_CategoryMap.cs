using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Mapping
{
    public class Book_CategoryMap : IEntityTypeConfiguration<Book_Category>
    {
        public void Configure(EntityTypeBuilder<Book_Category> builder)
        {
            builder.HasKey(t => new { t.BookID, t.CategoryID });
            builder.HasOne(p=>p.Book)
                   .WithMany(p=>p.book_Categories)
                   .HasForeignKey(p=>p.BookID);
            builder.HasOne(p => p.Category)
                   .WithMany(p => p.book_Categories)
                   .HasForeignKey(p => p.CategoryID);
        }
    }
}
