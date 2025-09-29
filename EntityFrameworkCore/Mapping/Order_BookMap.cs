using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace EntityFrameworkCore.Mapping
{
    public class Order_BookMap : IEntityTypeConfiguration<Order_Book>
    {
        public void Configure(EntityTypeBuilder<Order_Book> builder)
        {
            builder.HasKey(p => new { p.BookID, p.OrderID });

            builder.HasOne(p => p.Book)
                   .WithMany(p => p.Order_Book)
                   .HasForeignKey(f => f.BookID);

            builder.HasOne(p => p.Order)
                   .WithMany(t => t.Order_Books)
                   .HasForeignKey(f => f.OrderID);
        }
    }
}
