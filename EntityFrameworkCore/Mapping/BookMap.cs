using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace EntityFrameworkCore.Mapping
{
    public class BookMap : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("BookInfo");

            builder.HasKey(p => p.BookID);

            builder.Property(p => p.Title)
                   .IsRequired();

            builder.Property(p => p.Image)
                   .HasColumnType("image");
            builder.HasOne(p => p.Discount)
                   .WithOne(p => p.Book)
                   .HasForeignKey<Discount>(p => p.BookID);
        }
    }
}
