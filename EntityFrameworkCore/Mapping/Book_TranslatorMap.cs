using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Mapping
{
    public class Book_TranslatorMap : IEntityTypeConfiguration<Translator_Book>
    {
        public void Configure(EntityTypeBuilder<Translator_Book> builder)
        {
            builder.
                HasKey(tb => new { tb.BookID, tb.TranslatorID });
                builder.HasOne(b=> b.Book)
                       .WithMany(b=>b.Translator_Books)
                       .HasForeignKey(b=> b.BookID);
            builder.HasOne(b => b.Translator)
                       .WithMany(b => b.Translator_Books)
                       .HasForeignKey(b => b.TranslatorID);

        }
    }
}
