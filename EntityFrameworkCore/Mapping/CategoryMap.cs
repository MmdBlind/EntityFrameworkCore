using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace EntityFrameworkCore.Mapping
{
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData
            (
                    new Category { CategoryID = 1, CategoryName = "هنر" },
                    new Category { CategoryID = 2, CategoryName = "عمومی" },
                    new Category { CategoryID = 3, CategoryName = "دانشگاهی" }
            );
            builder.HasOne(c => c.category) // هر Category می‌تونه یک Parent داشته باشه
                   .WithMany(c => c.categories) // هر Parent می‌تونه چند تا Child داشته باشه
                   .HasForeignKey(c => c.ParentCategoryID) // این FK هست
                   .OnDelete(DeleteBehavior.Restrict); // تا خطای چرخه Cascade نگیری
        }
    }
}
