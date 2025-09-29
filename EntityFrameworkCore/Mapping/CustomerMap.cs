using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace EntityFrameworkCore.Mapping
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasOne(c => c.City1)
                   .WithMany(c => c.Customers1)
                   .HasForeignKey("City1CityID")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.City2)
                   .WithMany(c => c.Customers2)
                   .HasForeignKey("City2CityID")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.FirstName)
                   .HasColumnName("FName")
                   .HasColumnType("nvarchar(20)");

            builder.Property(c => c.LastName)
                   .HasColumnName("LName")
                   .HasMaxLength(100);

            builder.HasOne(p => p.City1)
                   .WithMany(t => t.Customers1)
                   .HasForeignKey(f => f.cityID1);

            builder.HasOne(p => p.City2)
                   .WithMany(t => t.Customers2)
                   .HasForeignKey(f => f.cityID2);

            builder.Ignore(c => c.Age);
        }
    }
}
