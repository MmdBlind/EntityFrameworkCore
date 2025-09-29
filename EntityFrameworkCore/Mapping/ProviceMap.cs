using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace EntityFrameworkCore.Mapping
{
    public class ProviceMap : IEntityTypeConfiguration<Provice>
    {
        public void Configure(EntityTypeBuilder<Provice> builder)
        {
            builder.Property(c => c.ProviceID)
                   .ValueGeneratedNever();
        }
    }
}
