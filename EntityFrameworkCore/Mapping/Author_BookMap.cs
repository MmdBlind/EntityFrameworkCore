using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Mapping
{
    public class Author_BookMap : IEntityTypeConfiguration<Author_Book>
    {
        public void Configure(EntityTypeBuilder<Author_Book> builder)
        {
            builder.HasKey(p => new { p.BookID, p.AuthorID });

            builder.HasOne(p => p.Book)
                   .WithMany(b => b.Author_Book)
                   .HasForeignKey(p => p.BookID);

            builder.HasOne(p => p.Author)
                   .WithMany(a => a.Author_Book)
                   .HasForeignKey(p => p.AuthorID);
        }
    }
}