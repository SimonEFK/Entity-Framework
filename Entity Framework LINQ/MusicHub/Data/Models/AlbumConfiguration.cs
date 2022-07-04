using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MusicHub.Data.Models
{
    public class AlbumConfiguration:IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(x=>x.Name)
                .IsRequired()
                .HasMaxLength(40);

           


        }
    }
}